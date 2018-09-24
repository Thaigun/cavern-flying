using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace CavernWars
{
    /// <summary>
    /// Party means one match, starting from the actual lobby where the players connect
    /// to the host and wait for the match to start and all the way to the actual game
    /// where the same players have joined.
    /// 
    /// Keeps track of the players that have joined the party, who is the host and other
    /// things related to keeping track of the state of party.
    /// </summary>
    public class PartyManager : MonoBehaviour
    {
        [SerializeField]
        private LobbyPlayerList _lobbyPlayerList;

        [SerializeField]
        private TMP_InputField _nameInput;

        [SerializeField, Tooltip("Which map is to be loaded when the game starts.")]
        private int _map = 1;

        private NetworkInterface _network;

        public static PartyManager Instance { get; private set; }

        private Host LocalHost { get; set; }
        public bool IsHost
        {
            get
            {
                return LocalHost != null;
            }
            set
            {
                if (value && LocalHost == null)
                {
                    LocalHost = new Host();
                }
                else if (!value)
                {
                    LocalHost = null;
                }
            }
        }
        // Connection id to the host of the game.
        public int HostConnectionId { get; private set; }
        public MatchStatus PartyStatus { get; private set; }
        public List<Player> Players { get; private set; }
        public string YourName { get; private set; }
        public int Map { get; private set; }

        // Use this for initialization
        void Awake()
        {
            // We destroy the existing instance and keep the new one because if we return to the main 
            // menu, there is no need to keep any data that has been stored to the party manager as 
            // returning to the main menu means the party has ended.
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }
            Instance = this;
        }

        private void Start()
        {
            _network = NetworkInterface.Instance;
            _network.connectionResponseDel += OnConnectionResponse;
            _network.disconnectDel += OnDisconnect;
            _network.lobbyUpdateDel += OnLobbyUpdate;
            _network.playerInfoDel += OnPlayerInfo;
            _network.matchStatusDel += OnMatchStatus;

            Players = new List<Player>();

            ResetParty();
        }

        private void Update()
        {
            if (PartyStatus == MatchStatus.WAITING)
            {
                if (IsHost && Players.All(plr => plr.Ready))
                {
                    MatchStatusMessage statusMsg = new MatchStatusMessage()
                    {
                        matchStatus = (int)MatchStatus.IN_PROGRESS
                    };
                    _network.SendToAllConnected(MessageType.MATCH_STATUS, statusMsg);
                    PartyStatus = MatchStatus.IN_PROGRESS;
                    CancelInvoke("SendLobbyUpdate");
                }
            }
        }

        private void OnDestroy()
        {
            CloseParty();
        }

        private void ResetParty()
        {
            Players.Clear();
            HostConnectionId = -1;
            PartyStatus = MatchStatus.LOBBY;
            IsHost = false;
            Map = -1;
        }

        private Player GetPlayerWithConnectionId(int connectionId)
        {
            return Players.Find(plr => plr.ConnectionId == connectionId);
        }

        private void OnLobbyUpdate(MessageContainer container)
        {
            LobbyUpdateMessage lobbyMsg = container.Message as LobbyUpdateMessage;
            
            // Update the players list based on the lobby update message
            for (int i = 0; i < lobbyMsg.players.Length; i++)
            {
                LobbyPlayerInfo msgPlr = lobbyMsg.players[i];
                if (!Players.Any(plr => plr.Name.Equals(msgPlr.name)))
                {
                    int connectionId = msgPlr.isHost ? HostConnectionId : -1;
                    Players.Add(new Player(msgPlr.name, msgPlr.ip, msgPlr.port, connectionId, msgPlr.isHost, msgPlr.name.Equals(YourName)));
                }
            }
            List<Player> toBeRemoved = new List<Player>();
            foreach (Player plr in Players)
            {
                if (!lobbyMsg.players.Any(p => p.name == plr.Name))
                {
                    toBeRemoved.Add(plr);
                }
            }
            foreach(Player plr in toBeRemoved)
            {
                Players.Remove(plr);
            }

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                _lobbyPlayerList.UpdateAllPlayers(Players);

                if (lobbyMsg.map > 0)
                {
                    LoadMatchScene(lobbyMsg.map);
                }
            }
        }

        private void OnPlayerInfo(MessageContainer container)
        {
            if (!IsHost)
            {
                return;
            }

            // If the player has not been added yet, do it now.
            PlayerInfoMessage plrInfo = container.Message as PlayerInfoMessage;
            if (!Players.Any(plr => plr.Name.Equals(plrInfo.name)))
            {
                int port;
                string address = _network.GetConnectionIp(container.ConnectionId, out port);
                Players.Add(new Player(plrInfo.name, address, port, container.ConnectionId, false, false));
                _lobbyPlayerList.UpdateAllPlayers(Players);
                // The update is sent every second anyway, no need to send it separately here.
                return;
            }
        }

        private void OnConnectionResponse(int connectionId)
        {
            if (connectionId == HostConnectionId)
            {
                PlayerInfoMessage infoMsg = new PlayerInfoMessage();
                infoMsg.name = _nameInput.text;
                _network.Send(MessageType.PLAYER_INFO, infoMsg, connectionId);
            }

            if (PartyStatus == MatchStatus.WAITING)
            {
                if (!IsHost)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    // Clients have to wait for all connections to other clients to be established
                    if (_network.ConnectionIds.Count == Players.Count && sceneName.Equals("Map" + _map))
                    {
                        SceneLoadedMessage sceneMsg = new SceneLoadedMessage()
                        {
                            sceneLoaded = _map
                        };
                        _network.Send(MessageType.SCENE_LOADED, sceneMsg, HostConnectionId);
                    }
                }
            }
        }

        private void OnDisconnect(int connectionId)
        {
            if (connectionId == HostConnectionId)
            {
                // If the host is lost, cannot play anymore
                // TODO: One of the clients becomes the host.
                SceneManager.LoadScene(0);
            }
            else if (IsHost)
            {
                Players.RemoveAll(plr => plr.ConnectionId == connectionId);
            }
        }

        private void OnGameSceneLoaded(MessageContainer container)
        {
            Player player = GetPlayerWithConnectionId(container.ConnectionId);
            SceneLoadedMessage sceneMsg = container.Message as SceneLoadedMessage;
            if (sceneMsg.sceneLoaded == _map)
            {
                player.Ready = true;
            }
        }

        private void OnMatchStatus(MessageContainer container)
        {
            MatchStatusMessage statusMsg = container.Message as MatchStatusMessage;
            PartyStatus = (MatchStatus)statusMsg.matchStatus;
        }

        public void JoinLobby(string ip, int port, string yourName)
        {
            _network.OpenSocket();
            HostConnectionId = _network.ConnectToIP(ip, port);
            YourName = yourName;
        }

        public void StartHosting(string yourName)
        {
            _network.OpenSocket();
            IsHost = true;
            InvokeRepeating("SendLobbyUpdate", 1f, 1f);
            YourName = yourName;
            Players.Add(new Player(YourName, "", -1, -1, true, true));
            _lobbyPlayerList.UpdateAllPlayers(Players);
        }

        public void StartMatch()
        {
            PartyStatus = MatchStatus.WAITING;
            LoadMatchScene(_map); // TODO: option to switch between maps.
        }

        public void SendLobbyUpdate()
        {
            LobbyUpdateMessage lobbyMsg = new LobbyUpdateMessage();
            if (PartyStatus == MatchStatus.LOBBY)
            {
                lobbyMsg.map = -1;
            }
            else if (PartyStatus == MatchStatus.WAITING)
            {
                lobbyMsg.map = _map;
            }
            else
            {
                return;
            }
            lobbyMsg.players = new LobbyPlayerInfo[Players.Count];
            for (int i = 0; i < Players.Count; i++)
            {
                lobbyMsg.players[i] = new LobbyPlayerInfo(Players[i]);
            }

            _network.SendToAllConnected(MessageType.LOBBY_UPDATE, lobbyMsg);
        }

        public void CloseParty()
        {
            _network.CloseConnections();
            ResetParty();
        }

        public void LoadMatchScene(int map)
        {
            _map = map;
            // Host is already connected to everyone
            if (!this.IsHost)
            {
                foreach (Player player in Players)
                {
                    // Open connections to other clients, host is already connected
                    // The player whose name comes first when ordered, requests the
                    // connection,
                    if (!player.IsHost && !player.IsYou)
                    {
                        player.ConnectionId = _network.ConnectToIP(player.Ip, player.Port);
                    }
                }
            }

            SceneManager.LoadScene("Map" + map);
            PartyStatus = MatchStatus.WAITING;
        }        
    }
}
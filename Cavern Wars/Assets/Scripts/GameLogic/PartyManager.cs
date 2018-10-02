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

        [SerializeField]
        private MatchHost _host;
        
        private int _map = 1;
        private bool _sceneLoadedSent;
        private NetworkInterface _network;
        private List<Player> _players;

        public static PartyManager Instance { get; private set; }
        
        public bool IsHost
        {
            get
            {
                return _host.gameObject.activeSelf;
            }
            set
            {
                _host.gameObject.SetActive(value);
            }
        }

        public MatchHost Host { get { return _host; } }

        // Connection id to the host of the game.
        public int HostConnectionId { get; private set; }
        public MatchStatus PartyStatus { get; private set; }
        public List<Player> Players
        {
            get
            {
                if (IsHost)
                {
                    return Host.Players;
                }
                else
                {
                    if (_players == null)
                    {
                        _players = new List<Player>();
                    }
                    return _players;
                }
            }
        }
        public string YourName { get; private set; }
        public int Map { get; private set; }

        private void Start()
        {
            Instance = this;

            _network = NetworkInterface.Instance;
            _network.connectionResponseDel += OnConnectionResponse;
            _network.disconnectDel += OnDisconnect;
            _network.lobbyUpdateDel += OnLobbyUpdate;
            _network.matchStatusDel += OnMatchStatus;

            ResetParty();
        }

        private void Update()
        {
            if (!_sceneLoadedSent)
            {
                TrySendSceneLoaded();
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

        public Player GetPlayerWithConnectionId(int connectionId)
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
                // Host's party manager uses the player list of the host.
                if (!Players.Any(plr => plr.Name.Equals(msgPlr.name)))
                {
                    // For players other than host, this will be updated when connecting them in the beginning of the match.
                    int connectionId = HostConnectionId;
                    Players.Add(new Player(msgPlr.name, msgPlr.ip, msgPlr.port, connectionId, msgPlr.isHost, msgPlr.name.Equals(YourName)));
                }
            }
            List<Player> toBeRemoved = new List<Player>();
            foreach (Player plr in Players)
            {
                if (!lobbyMsg.players.Any(p => p.name.Equals(plr.Name)))
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

        private void OnConnectionResponse(int connectionId)
        {
            if (connectionId == HostConnectionId)
            {
                PlayerInfoMessage infoMsg = new PlayerInfoMessage();
                infoMsg.name = _nameInput.text;
                _network.Send(MessageType.PLAYER_INFO, infoMsg, connectionId);
            }
        }

        private void TrySendSceneLoaded()
        {
            if (PartyStatus == MatchStatus.WAITING)
            {
                if (!IsHost)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    // Clients have to wait for all connections to other clients to be established
                    if (_network.ConnectionIds.Count + 1 == Players.Count && sceneName.Equals("Map" + _map))
                    {
                        SceneLoadedMessage sceneMsg = new SceneLoadedMessage()
                        {
                            sceneLoaded = _map
                        };
                        _network.Send(MessageType.SCENE_LOADED, sceneMsg, HostConnectionId);
                        _sceneLoadedSent = true;
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
            IsHost = true;

            JoinLobby("local", -1, yourName);
        }       

        public void CloseParty()
        {
            if (_network)
            {
                _network.CloseConnections();
                ResetParty();
            }
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
                    // connection, so we don't get duplicate connections.
                    if (!player.IsHost && !player.IsYou && player.Name.CompareTo(YourName) < 0)
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
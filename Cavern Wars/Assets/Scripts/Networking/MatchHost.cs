using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CavernWars
{
    public class MatchHost : MonoBehaviour
    {
        // TODO: Refactor this whole class

        [SerializeField]
        private float _packetsPerSecond = 15f;

        [SerializeField]
        private float _maxHealth = 100f;

        [SerializeField]
        private float _deathTime = 3f;

        [SerializeField, Tooltip("Which map is to be loaded when the game starts.")]
        private int _map = 1;

        private float _packetInterval;
        private float _lastPacketTime;
        private MatchStatus _matchStatus;

        private PlayersHealthMessage _nextHealthMessage;
        private Dictionary<string, float> _playerDeathTime;
        private bool Started { get; set; }

        public List<Player> Players { get; set; }

        private void Start()
        {
            Players = new List<Player>();

            _matchStatus = MatchStatus.LOBBY;
            InvokeRepeating("SendLobbyUpdate", 1f, 1f);

            NetworkInterface.Instance.playerInfoDel += OnPlayerInfo;
            NetworkInterface.Instance.sceneLoadedDel += OnGameSceneLoaded;
        }

        private void Update()
        {
            if (_matchStatus == MatchStatus.WAITING && Players.All(plr => plr.IsHost || plr.Ready))
            {
                MatchStatusMessage statusMsg = new MatchStatusMessage()
                {
                    matchStatus = (int)MatchStatus.IN_PROGRESS
                };
                NetworkInterface.Instance.SendToAllConnected(MessageType.MATCH_STATUS, statusMsg);
                _matchStatus = MatchStatus.IN_PROGRESS;
                CancelInvoke("SendLobbyUpdate");
                PrepareForMatchStart();
            }

            if (Started && NetworkInterface.Instance && Time.time - _lastPacketTime > _packetInterval)
            {
                NetworkInterface.Instance.SendToAllConnected(MessageType.HEALTH_MESSAGE, _nextHealthMessage, NetworkInterface.Instance.UnreliableChannel);
                _lastPacketTime = Time.time;
            }
        }

        private void OnDisable()
        {
            CancelInvoke("SendLobbyUpdate");
            Players.Clear();
            _matchStatus = MatchStatus.LOBBY;
        }

        private void OnPlayerHit(MessageContainer msgContainer)
        {
            PlayerHitMessage hitMessage = msgContainer.Message as PlayerHitMessage;
            for (int i = 0; i < hitMessage.hitPlayers.Length; i++)
            {
                for (int j = 0; j < _nextHealthMessage.playerNames.Length; j++)
                {
                    if (hitMessage.hitPlayers[i].Equals(_nextHealthMessage.playerNames[j]))
                    {
                        _nextHealthMessage.healths[j] -= hitMessage.damages[i];
                        if (_nextHealthMessage.healths[j] <= 0f && !_playerDeathTime.ContainsKey(_nextHealthMessage.playerNames[j]))
                        {
                            _playerDeathTime.Add(_nextHealthMessage.playerNames[j], Time.time);
                        }
                        break;
                    }
                }
            }
        }

        private void OnGameSceneLoaded(MessageContainer container)
        {
            Player player = Players.Find(plr => plr.ConnectionId == container.ConnectionId);
            SceneLoadedMessage sceneMsg = container.Message as SceneLoadedMessage;
            if (sceneMsg.sceneLoaded == _map)
            {
                player.Ready = true;
            }
        }

        private void OnGameUpdate(MessageContainer msgContainer)
        {
            GameUpdateMessage msg = msgContainer.Message as GameUpdateMessage;
            if (msg.alive)
            {
                string playerName = PartyManager.Instance.GetPlayerWithConnectionId(msgContainer.ConnectionId).Name;
                float dTime;
                if (_playerDeathTime.TryGetValue(playerName, out dTime))
                {
                    if (Time.time - dTime > _deathTime)
                    {
                        AllowPlayerSpawn(playerName);
                    }
                }
            }
        }

        void AllowPlayerSpawn(string name)
        {
            _playerDeathTime.Remove(name);
            for (int i = 0; i < _nextHealthMessage.playerNames.Length; i++)
            {
                if (_nextHealthMessage.playerNames[i].Equals(name))
                {
                    _nextHealthMessage.healths[i] = _maxHealth;
                }
                break;
            }
        }

        private void ResetHitMessage()
        {
            _nextHealthMessage = new PlayersHealthMessage();
            _nextHealthMessage.maxHealth = _maxHealth;
            _nextHealthMessage.playerNames = PartyManager.Instance.Players.Select(player => player.Name).ToArray();
            _nextHealthMessage.healths = new float[_nextHealthMessage.playerNames.Length];
        }

        private void PrepareForMatchStart()
        {
            _lastPacketTime = Time.time;
            _packetInterval = 1f / _packetsPerSecond;
            _playerDeathTime = new Dictionary<string, float>();
            ResetHitMessage();
            for (int i = 0; i < _nextHealthMessage.healths.Length; i++)
            {
                _nextHealthMessage.healths[i] = GameController.Instance.spawnFromBeginning ? _maxHealth : -1;
            }

            NetworkInterface.Instance.playerHitDel += OnPlayerHit;
            NetworkInterface.Instance.gameUpdateDel += OnGameUpdate;

            Started = true;
        }

        private void OnPlayerInfo(MessageContainer container)
        {
            // If the player has not been added yet, do it now.
            PlayerInfoMessage plrInfo = container.Message as PlayerInfoMessage;
            if (!Players.Any(plr => plr.Name.Equals(plrInfo.name)))
            {
                int port;
                string address = NetworkInterface.Instance.GetConnectionIp(container.ConnectionId, out port);
                Players.Add(new Player(plrInfo.name, address, port, container.ConnectionId, container.ConnectionId == -1, container.ConnectionId == -1));
                // The update is sent every second anyway, no need to send it separately here.
                return;
            }
        }

        public void SendLobbyUpdate()
        {
            LobbyUpdateMessage lobbyMsg = new LobbyUpdateMessage();
            if (_matchStatus == MatchStatus.LOBBY)
            {
                lobbyMsg.map = -1;
            }
            else if (_matchStatus == MatchStatus.WAITING)
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

            NetworkInterface.Instance.SendToAllConnected(MessageType.LOBBY_UPDATE, lobbyMsg);
        }

        public void StartMatch()
        {
            _matchStatus = MatchStatus.WAITING;
            Players.Find(plr => plr.IsHost).Ready = true;
        }
    }
}

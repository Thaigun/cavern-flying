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

        private float _packetInterval;
        private float _lastPacketTime;

        private PlayersHealthMessage _nextHealthMessage;

        private Dictionary<string, float> _playerDeathTime;

        private void Start()
        {
            _lastPacketTime = Time.time;
            _packetInterval = 1f / _packetsPerSecond;
            _playerDeathTime = new Dictionary<string, float>();
            ResetHitMessage();
            for (int i = 0; i < _nextHealthMessage.healths.Length; i++)
            {
                _nextHealthMessage.healths[i] = _maxHealth;//-1;
            }

            NetworkInterface.Instance.playerHitDel += OnPlayerHit;
            NetworkInterface.Instance.gameUpdateDel += OnGameUpdate;
        }

        private void Update()
        {
            if(NetworkInterface.Instance && Time.time - _lastPacketTime > _packetInterval)
            {
                NetworkInterface.Instance.SendToAllConnected(MessageType.HEALTH_MESSAGE, _nextHealthMessage, NetworkInterface.Instance.UnreliableChannel);
                _lastPacketTime = Time.time;
            }
        }

        private void OnPlayerHit(MessageContainer msgContainer)
        {
            PlayerHitMessage hitMessage = msgContainer.Message as PlayerHitMessage;
            for (int i = 0; i < hitMessage.hitPlayers.Length; i++)
            {
                for (int j = 0; j < _nextHealthMessage.playerNames.Length; j++)
                {
                    if (hitMessage.hitPlayers[i] == _nextHealthMessage.playerNames[i])
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
                if (_nextHealthMessage.playerNames[i] == name)
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
    }
}

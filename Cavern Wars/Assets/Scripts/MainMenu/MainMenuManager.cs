using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

namespace CavernWars
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _ipInput;

        [SerializeField]
        private TMP_InputField _portInput;

        [SerializeField]
        private TMP_InputField _nameInput;

        [SerializeField]
        private Text _yourIpText;

        [SerializeField]
        private LobbyPlayerList _lobbyPlayerList;

        // Only for convenience; to not have to write the whole thing every time.
        private NetworkInterface _network;

        // Use this for initialization
        void Start()
        {
            _network = NetworkInterface.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (_network.ReceivedMessages.Count > 0)
            {
                MessageBase recMsg = _network.ReceivedMessages.Peek();

                LobbyUpdateMessage lobbyMsg = recMsg as LobbyUpdateMessage;
                if (lobbyMsg != null)
                {
                    _network.ReceivedMessages.Dequeue();
                    OnLobbyUpdateMessage(lobbyMsg);
                }
            }
        }

        private void OnLobbyUpdateMessage(LobbyUpdateMessage lobbyMsg)
        {
            List<string> names = new List<string>();
            List<string> ips = new List<string>();
            List<int> ids = new List<int>();
            foreach(LobbyPlayerInfo playerInfo in lobbyMsg.players)
            {
                names.Add(playerInfo.name);
                ips.Add(playerInfo.ip);
                ids.Add(playerInfo.id);
            }
            _lobbyPlayerList.UpdateAllPlayers(lobbyMsg.players.Length, names, ips, ids, _nameInput.text);

            if (lobbyMsg.map > 0)
            {
                LoadMatchScene(lobbyMsg.map);
            }
        }

        public void HostClicked()
        {
            _network.OpenSocket();
            GlobalSettings.IsHost = true;
            _yourIpText.text = _network.GetIP();
        }

        public void JoinClicked()
        {
            _network.OpenSocket();
            _network.ConnectToIP(_ipInput.text, int.Parse(_portInput.text));
        }

        public void StartGameClicked()
        {
            MatchStatusMessage msg = new MatchStatusMessage()
            {
                matchStatus = 1
            };
            _network.SendToAllConnected(MessageType.MATCH_STATUS, msg);
        }

        public void CancelLobbyClicked()
        {
            _network.CloseConnections();
            GlobalSettings.IsHost = false;
        }

        public void QuitClicked()
        {
            Application.Quit();
        }

        public void LoadMatchScene(int map)
        {
            throw new System.NotImplementedException("Load match scene not implemented");
        }
    }
}
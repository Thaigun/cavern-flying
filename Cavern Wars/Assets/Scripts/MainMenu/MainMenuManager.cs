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
        private TMP_InputField _nameInput;

        [SerializeField]
        private TMP_InputField _ipInput;

        [SerializeField]
        private TMP_InputField _portInput;

        [SerializeField]
        private Text _yourIpText;

        public void HostClicked()
        {
            PartyManager.Instance.StartHosting(_nameInput.text);
            _yourIpText.text = NetworkInterface.Instance.GetYourIp();
        }

        public void JoinClicked()
        {
            PartyManager.Instance.JoinLobby(_ipInput.text, int.Parse(_portInput.text), _nameInput.text);
        }

        public void StartGameClicked()
        {
            PartyManager.Instance.Host.StartMatch();
        }

        public void CancelLobbyClicked()
        {
            PartyManager.Instance.CloseParty();
        }

        public void QuitClicked()
        {
            Application.Quit();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
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

        private void Start()
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic, true);
        }

        private void Update()
        {
            // Allow quickly typing in the link local address of my dev machine.
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
            {
                _ipInput.text = "169.254.211.28";
            }
        }

        public void PlayButtonSound()
        {
            AudioManager.Instance.PlayClip(AudioManager.Instance.buttonClick, false);
        }

        public void HostClicked()
        {
            PartyManager.Instance.StartHosting(_nameInput.text);
            StringBuilder sb = new StringBuilder();
            int port;
            foreach (IPAddress addr in NetworkInterface.Instance.GetYourIp(out port))
            {
                string address = addr.ToString();
                if (addr.IsIPv6LinkLocal || address.StartsWith("169.254."))
                {
                    sb.AppendLine("Link-local: " + address + ":" + port);
                }
                else if (address.StartsWith("10.") || address.StartsWith("192.168."))
                {
                    sb.AppendLine("LAN: " + address + ":" + port);
                }
                else
                {
                    sb.AppendLine(address + ":" + port);
                }
            }
            _yourIpText.text = sb.ToString();
            PlayButtonSound();
        }

        public void JoinClicked()
        {
            PartyManager.Instance.JoinLobby(_ipInput.text, int.Parse(_portInput.text), _nameInput.text);
            PlayButtonSound();
        }

        public void StartGameClicked()
        {
            PartyManager.Instance.Host.StartMatch();
            PlayButtonSound();
        }

        public void CancelLobbyClicked()
        {
            PartyManager.Instance.CloseParty();
            PlayButtonSound();
        }

        public void QuitClicked()
        {
            PlayButtonSound();
            Application.Quit();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CavernWars
{
    public class HelpPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;

        [SerializeField]
        private Text _helpText;

        public bool Active { get; private set; }

        private void Start()
        {
        }

        public void Show(string text = "")
        {
            _panel.SetActive(true);
            SetText(text);
            Active = true;
        }

        public void Hide()
        {
            _panel.SetActive(false);
            Active = false;
        }

        public void SetText(string text)
        {
            _helpText.text = text;
        }
    }
}
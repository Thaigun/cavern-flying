using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CavernWars
{
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInput : MonoBehaviour
    {
        [SerializeField]
        private GameObject _helpBubble;

        public void SetHelpBubbleActive()
        {
            string nameText = GetComponent<TMP_InputField>().text;
            _helpBubble.SetActive(string.IsNullOrEmpty(nameText));
        }
    }
}
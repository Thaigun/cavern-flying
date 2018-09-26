using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

namespace CavernWars
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerName;

        [SerializeField]
        private TMP_Text _hpText;

        [SerializeField]
        private int _maxHealthLength = 20;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private void Start()
        {
            _originalPosition = transform.position - transform.parent.position;
            _originalRotation = transform.rotation;

            PlayerName = GetComponentInParent<PlayerState>().NetworkPlayer.Name;
        }

        private void Update()
        {
            transform.rotation = _originalRotation;
            transform.position = transform.parent.position + _originalPosition;
        }

        private string PlayerName
        {
            get
            {
                return _playerName.GetParsedText();
            }
            set
            {
                _playerName.SetText(value);
            }
        }
        
        // The ugliest hack of this game, how the healthbar is implemented:
        public void SetHealth(float hp, float maxHp)
        {
            int hpBarCount = Mathf.CeilToInt((hp / maxHp) * _maxHealthLength);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hpBarCount; i++)
            {
                sb.Append('|');
            }
            _hpText.text = sb.ToString();
        }
    }
}
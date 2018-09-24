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

        private TMP_Text _hpText;
        private int _maxHealthLength;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private void Start()
        {
            _originalPosition = transform.position - transform.parent.position;
            _originalRotation = transform.rotation;
            _hpText = GetComponent<TMP_Text>();
            // The ugliest hack of this game:
            _maxHealthLength = _hpText.text.Length;
        }

        private void Update()
        {
            transform.rotation = _originalRotation;
            transform.position = transform.parent.position + _originalPosition;
        }

        public string PlayerName
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
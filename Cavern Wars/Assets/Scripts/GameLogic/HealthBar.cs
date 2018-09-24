using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private float _fullHealth;

        public string PlayerName { get; set; }

        public void SetHealth(float hp)
        {

        }
    }
}
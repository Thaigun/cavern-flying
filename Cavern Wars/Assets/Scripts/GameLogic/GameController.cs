using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        public Dictionary<string, HealthBar> Healthbars { get; private set; }

        private void Awake()
        {
            Instance = this;
            Healthbars = new Dictionary<string, HealthBar>();
        }

        public void AddHealthbar(string playerName, HealthBar hpBar)
        {
            Healthbars.Add(playerName, hpBar);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    class PlayerHealthUpdater : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            NetworkInterface.Instance.playerHealthDel += UpdatePlayersHealth;
        }

        private void UpdatePlayersHealth(MessageContainer msgContainer)
        {
            PlayersHealthMessage healthMessage = msgContainer.Message as PlayersHealthMessage;
            for (int i = 0; i < healthMessage.healths.Length; i++)
            {
                HealthBar healthBar;
                if (GameController.Instance.Healthbars.TryGetValue(healthMessage.playerNames[i], out healthBar))
                {
                    healthBar.SetHealth(healthMessage.healths[i]);
                }
            }
        }
    }
}
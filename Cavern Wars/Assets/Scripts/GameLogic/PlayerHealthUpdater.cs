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
            if (NetworkInterface.Instance && PartyManager.Instance)
            {
                HealthBar playerHpBar = GameController.Instance.Player.GetComponentInChildren<HealthBar>();
                GameController.Instance.AddHealthbar(PartyManager.Instance.YourName, playerHpBar);
                playerHpBar.PlayerName = PartyManager.Instance.YourName;

                var matChanger = GameController.Instance.Player.GetComponent<MaterialChanger>();
                if (matChanger != null)
                {
                    matChanger.UpdateColor(playerHpBar.PlayerName);
                }

                NetworkInterface.Instance.playerHealthDel += UpdatePlayersHealth;
            }
        }

        private void UpdatePlayersHealth(MessageContainer msgContainer)
        {
            PlayersHealthMessage healthMessage = msgContainer.Message as PlayersHealthMessage;
            for (int i = 0; i < healthMessage.healths.Length; i++)
            {
                HealthBar healthBar;
                if (GameController.Instance.Healthbars.TryGetValue(healthMessage.playerNames[i], out healthBar))
                {
                    healthBar.SetHealth(healthMessage.healths[i], healthMessage.maxHealth);
                }

                if (healthMessage.playerNames[i].Equals(PartyManager.Instance.YourName))
                {
                    GameController.Instance.PlayerAliveFromServer(healthMessage.healths[i] > 0f);
                }
            }
        }
    }
}
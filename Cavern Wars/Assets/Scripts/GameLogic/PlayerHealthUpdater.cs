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
                NetworkInterface.Instance.playerHealthDel += UpdatePlayersHealth;
            }
        }

        private void UpdatePlayersHealth(MessageContainer msgContainer)
        {
            PlayersHealthMessage healthMessage = msgContainer.Message as PlayersHealthMessage;
            for (int i = 0; i < healthMessage.healths.Length; i++)
            {
                PlayerState updatePlr = GameController.Instance.GetPlayerWithName(healthMessage.playerNames[i]);
                if (updatePlr == null)
                {
                    continue;
                }
                updatePlr.HPBar.SetHealth(healthMessage.healths[i], healthMessage.maxHealth);
                if (!updatePlr.NetworkPlayer.IsYou)
                { 
                    if (healthMessage.healths[i] <= 0)
                    {
                        updatePlr.Despawn();
                    }
                    else
                    {
                        updatePlr.Spawn();
                    }
                }
                else
                {
                    GameController.Instance.PlayerAliveFromServer(healthMessage.healths[i] > 0f);
                }
            }
        }
    }
}
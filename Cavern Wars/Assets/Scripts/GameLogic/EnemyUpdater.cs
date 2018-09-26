using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class EnemyUpdater : MonoBehaviour
    {
        [SerializeField]
        private PlayerState _enemyPrefab;

        // Use this for initialization
        void Awake()
        {
            if (PartyManager.Instance == null || NetworkInterface.Instance == null)
            {
                return;
            }

            foreach (Player player in PartyManager.Instance.Players)
            {
                if (player.IsYou)
                {
                    GameController.Instance.LocalPlayer.NetworkPlayer = player;
                    GameController.Instance.LocalPlayer.UpdateColor();
                    continue;
                }
                PlayerState enemy = Instantiate(_enemyPrefab, transform);
                enemy.NetworkPlayer = player;
                enemy.UpdateColor();
                GameController.Instance.Enemies.Add(enemy);

                if (!GameController.Instance.spawnFromBeginning)
                {
                    enemy.gameObject.SetActive(false);
                }
            }

            NetworkInterface.Instance.gameUpdateDel += OnGameUpdate;
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnGameUpdate(MessageContainer msgContainer)
        {
            GameUpdateMessage msg = msgContainer.Message as GameUpdateMessage;
            PlayerState sender = GameController.Instance.GetEnemyWithConnectionId(msgContainer.ConnectionId);
            if (sender != null)
            {
                sender.ApplyNewState(msg);
            }
        }
    }
}
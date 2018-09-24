using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class EnemyUpdater : MonoBehaviour
    {
        [SerializeField]
        private MoveWithNetwork _enemyPrefab;

        private List<MoveWithNetwork> _enemies;

        // Use this for initialization
        void Awake()
        {
            _enemies = new List<MoveWithNetwork>();

            foreach (Player player in PartyManager.Instance.Players)
            {
                if (player.IsYou)
                {
                    continue;
                }
                MoveWithNetwork enemy = Instantiate(_enemyPrefab, transform);
                enemy.NetworkPlayer = player;
                _enemies.Add(enemy);
                HealthBar hpBar = enemy.GetComponentInChildren<HealthBar>(true);
                if (hpBar)
                {
                    hpBar.PlayerName = player.Name;
                    GameController.Instance.AddHealthbar(player.Name, hpBar);
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
            MoveWithNetwork sender = _enemies.Find(enemy => enemy.NetworkPlayer.ConnectionId == msgContainer.ConnectionId);
            if (sender != null)
            {
                sender.ApplyNewState(msg);
            }
        }
    }
}
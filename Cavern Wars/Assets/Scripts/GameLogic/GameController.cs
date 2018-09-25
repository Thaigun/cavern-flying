using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private MatchHost _host;

        [SerializeField]
        private HelpPanel _helpPanel;

        [SerializeField]
        private GameObject _player;

        [SerializeField]
        private Map _map;

        public GameObject Player { get { return _player; } }

        public static GameController Instance { get; private set; }

        public Dictionary<string, HealthBar> Healthbars { get; private set; }

        public bool WantsToLive { get; set; }

        private void Awake()
        {
            WantsToLive = true;
            Instance = this;
            Healthbars = new Dictionary<string, HealthBar>();
            if (PartyManager.Instance && PartyManager.Instance.IsHost)
            {
                _host.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _helpPanel.Active)
            {
                StartCoroutine(SpawnCountdown(3f));
            }
        }

        public void AddHealthbar(string playerName, HealthBar hpBar)
        {
            Healthbars.Add(playerName, hpBar);
        }

        public void PlayerAliveFromServer(bool alive)
        {
            if (!alive && Player.gameObject.activeInHierarchy)
            {
                Player.gameObject.SetActive(false);
                _helpPanel.Show("Respawn with [Space]");
            }
            else if (alive)
            {
                Player.gameObject.SetActive(true);
            }
        }

        private IEnumerator SpawnCountdown(float time)
        {
            float spawnTime = Time.time + time;
            float timeLeft;
            do
            {
                timeLeft = spawnTime - Time.time;
                _helpPanel.SetText("Respawning in " + timeLeft.ToString("F1") + "seconds");
                yield return null;
            } while (timeLeft > 0f);
            WantsToLive = true;
            _helpPanel.Hide();
            Player.transform.position = _map.GetRandomSpawnPoint().position;
        }
    }
}
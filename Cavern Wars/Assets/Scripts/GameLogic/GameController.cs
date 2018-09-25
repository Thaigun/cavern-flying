using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CavernWars
{
    class GameController : MonoBehaviour
    {
        [SerializeField]
        private MatchHost _host;

        [SerializeField]
        private HelpPanel _helpPanel;

        [SerializeField]
        private PlayerState _player;

        [SerializeField]
        private Map _map;
        
        public List<PlayerState> Enemies { get; private set; }

        public PlayerState LocalPlayer { get { return _player; } }

        public List<PlayerState> AllPlayers
        {
            get
            {
                List<PlayerState> combinedList = new List<PlayerState>(Enemies);
                combinedList.Add(LocalPlayer);
                return combinedList;
            }
        }

        public static GameController Instance { get; private set; }

        public bool WantsToLive { get; set; }

        private void Awake()
        {
            Enemies = new List<PlayerState>();
            WantsToLive = true;
            Instance = this;
            if (PartyManager.Instance && PartyManager.Instance.IsHost)
            {
                _host.gameObject.SetActive(true);
            }

            PlayerAliveFromServer(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _helpPanel.Active)
            {
                StartCoroutine(SpawnCountdown(3f));
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }

        public void PlayerAliveFromServer(bool alive)
        {
            if (!alive && LocalPlayer.gameObject.activeInHierarchy)
            {
                WantsToLive = false;
                LocalPlayer.gameObject.SetActive(false);
                _helpPanel.Show("Respawn with [Space]");
            }
            else if (alive)
            {
                LocalPlayer.gameObject.SetActive(true);
            }
        }

        public PlayerState GetPlayerWithName(string name)
        {
            return AllPlayers.Find(p => p.NetworkPlayer.Name.Equals(name));
        }

        public PlayerState GetEnemyWithName(string name)
        {
            return Enemies.Find(e => e.NetworkPlayer.Name.Equals(name));
        }

        public PlayerState GetEnemyWithConnectionId(int connId)
        {
            return Enemies.Find(e => e.NetworkPlayer.ConnectionId == connId);
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
            LocalPlayer.transform.position = _map.GetRandomSpawnPoint().position;
        }
    }
}
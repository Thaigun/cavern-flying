using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CavernWars
{
    public class LobbyPlayerList : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNamePrefab;

        private List<Text> _playerNames;

        private void Awake()
        {
            _playerNames = new List<Text>();
        }

        private void OnEnable()
        {
            UpdateAllPlayers(PartyManager.Instance.Players);
        }

        private void OnDisable()
        {
            UpdateAllPlayers(new List<Player>());
        }

        public void UpdateAllPlayers(IList<Player> players)
        {
            // This looks a bit messy but the idea is to avoid respawning the game objects
            // all the time. In the end, this is not called often enough for that to become an
            // issue but I decided to keep this anyway.
            
            // Destroy and remove players that are not in the new list of players.
            for (int i = _playerNames.Count-1; i >= players.Count; i--)
            {
                Destroy(_playerNames[i].gameObject);
                _playerNames.RemoveAt(i);
            }

            // Create as many game objects as are needed.
            for (int i = _playerNames.Count; i < players.Count; i++)
            {
                Text newPlayer = Instantiate(_playerNamePrefab, this.transform);
                _playerNames.Add(newPlayer);
            }

            if (players.Count != _playerNames.Count)
            {
                Debug.LogError("This code is messed up, take a look...");
            }

            for (int i = 0; i < players.Count; i++)
            {
                Text plr = _playerNames[i];
                plr.text = players[i].Name;
                plr.color = players[i].IsYou ? Color.blue : Color.white;
                plr.color = players[i].IsHost ? Color.red : plr.color;
            }
        }
    }
}
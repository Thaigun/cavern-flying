using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class LobbyPlayerList : MonoBehaviour
    {
        [SerializeField]
        private LobbyPlayer _playerPrefab;

        private List<LobbyPlayer> _players;

        private void Start()
        {
            _players = new List<LobbyPlayer>();
        }

        public void UpdateAllPlayers(int count, IList<string> names, IList<string> ips, IList<int> ids, string yourName)
        {
            // Destroy and remove players that are not in the new list of players.
            for (int i = _players.Count-1; i >= count; i--)
            {
                Destroy(_players[i]);
                _players.RemoveAt(i);
            }

            // Create as many game objects as are needed.
            for (int i = _players.Count; i < count; i++)
            {
                LobbyPlayer newPlayer = Instantiate(_playerPrefab, this.transform);
                _players.Add(newPlayer);
            }

            if (count != _players.Count)
            {
                Debug.LogError("This code is messed up, take a look...");
            }

            for (int i = 0; i < count; )
            {
                LobbyPlayer plr = _players[i];
                plr.SetData(names[i], ips[i], ids[i], names[i] == yourName);
            }
        }
    }
}
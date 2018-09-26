using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField]
        private ScoreRow _rowPrefab;

        private List<ScoreRow> _rows;
        
        void Start()
        {
            _rows = new List<ScoreRow>();
            foreach (var player in PartyManager.Instance.Players)
            {
                ScoreRow newRow = Instantiate(_rowPrefab, transform);
                newRow.SetData(player.Name, 0, 0);
                _rows.Add(newRow);
            }
            NetworkInterface.Instance.scoreDel += OnScoreMessage;
        }

        private void OnScoreMessage(MessageContainer msgContainer)
        {
            ScoreMessage msg = msgContainer.Message as ScoreMessage;
            for (int i = 0; i < msg.playerNames.Length; i++)
            {
                ScoreRow playerRow = _rows.Find(row => row.Name.Equals(msg.playerNames[i]));
                if (playerRow != null)
                {
                    playerRow.SetData(msg.playerNames[i], msg.kills[i], msg.deaths[i]); 
                }
                else
                {
                    Debug.LogError("Did not find a scoreboard row for player " + msg.playerNames[i]);
                }
            }
        }
    }
}
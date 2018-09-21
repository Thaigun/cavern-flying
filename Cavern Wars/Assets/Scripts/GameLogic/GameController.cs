using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class GameController : MonoBehaviour
    {

        [SerializeField]
        private Transform _playersContainer;

        private bool Started { get; set; }

        void Start()
        {

        }

        void Update()
        {
            if (!Started && PartyManager.Instance != null && PartyManager.Instance.PartyStatus == MatchStatus.IN_PROGRESS)
            {
                _playersContainer.gameObject.SetActive(true);
                Started = true;
            }
                
        }
    }
}
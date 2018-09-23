using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{

    public class EnemyUpdater : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            NetworkInterface.Instance.gameUpdateDel += OnGameUpdate;
    
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnGameUpdate(MessageContainer msgContainer)
        {

        }
    }
}
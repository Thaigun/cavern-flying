using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class Undestroyable : MonoBehaviour
    {
        private static Undestroyable _instance;

        // Use this for initialization
        void Awake()
        {
            if (_instance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
                _instance = this;
            }
        }

    }
}
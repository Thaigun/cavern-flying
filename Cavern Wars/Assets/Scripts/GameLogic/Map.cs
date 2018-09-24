using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _spawnPoints;

        public Transform GetRandomSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    [RequireComponent(typeof(Rigidbody2D))]
    class NetworkBullet : MonoBehaviour
    {
        public Rigidbody2D RigidbodyComponent { get; private set; }

        public int Id { get; set; }

        // Use this for initialization
        void Start()
        {
            RigidbodyComponent = GetComponent<Rigidbody2D>();
        }

        public void Despawn(BulletDespawnType type)
        {
            // TODO: Play effects
            Destroy(gameObject);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    [RequireComponent(typeof(Rigidbody2D))]
    class NetworkBullet : MonoBehaviour
    {
        private Rigidbody2D _rigidBody;

        public Rigidbody2D RigidbodyComponent
        {
            get
            {
                if (_rigidBody == null)
                {
                    _rigidBody = GetComponent<Rigidbody2D>();
                }
                return _rigidBody;
            }
        }

        public int Id { get; set; }

        public void Despawn(BulletDespawnType type)
        {
            // TODO: Play effects
            Destroy(gameObject);
        }
    }
}
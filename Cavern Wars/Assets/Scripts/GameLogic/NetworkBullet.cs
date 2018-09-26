using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    [RequireComponent(typeof(Rigidbody2D))]
    class NetworkBullet : MonoBehaviour
    {
        [SerializeField]
        private GameObject _smallExplosionPrefab;

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
            if (type == BulletDespawnType.NONE)
            {
                return;
            }

            if (type == BulletDespawnType.SMALL_HIT)
            {
                AudioManager.Instance.PlayClip(AudioManager.Instance.wallHit, false);
                var explosion = Instantiate(_smallExplosionPrefab, transform.position, _smallExplosionPrefab.transform.rotation);
                Destroy(explosion, 1f);
            }
            else if (type == BulletDespawnType.BIG_HIT)
            {
                AudioManager.Instance.PlayClip(AudioManager.Instance.shipHit, false);
                var explosion = Instantiate(_smallExplosionPrefab, transform.position, _smallExplosionPrefab.transform.rotation);
                Destroy(explosion, 1f);
            }

            Destroy(this.gameObject);
        }
    }
}
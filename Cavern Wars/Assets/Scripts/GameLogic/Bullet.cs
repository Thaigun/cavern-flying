using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    enum BulletDespawnType
    {
        NONE, SILENT, SMALL_HIT, BIG_HIT
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float _timeToLive = 10f;

        [SerializeField]
        private float _damage = 10f;

        [SerializeField]
        private GameObject _smallExplosionPrefab;

        private Vector2 _direction;

        private float _spawnTime;

        private Rigidbody2D _rigidBody;

        public int Id { get; set; }

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

        // Use this for initialization
        void Start()
        {
            _spawnTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _spawnTime > _timeToLive)
            {
                DespawnBullet(Id, BulletDespawnType.SILENT);
            }
        }

        /// <summary>
        /// Shoots the bullet towards target
        /// </summary>
        /// <param name="direction">Represents direction and speed of the bullet</param>
        public void ShootTo(Vector3 direction)
        {
            GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
        
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (GlobalEvents.projectileHitDel != null)
                {
                    GlobalEvents.projectileHitDel(collider.GetComponent<MoveWithNetwork>().NetworkPlayer.Name, _damage);
                }
            }
        }

        private void DespawnBullet(int id, BulletDespawnType type)
        {
            if (type == BulletDespawnType.NONE)
            {
                return;
            }

            if (GlobalEvents.projectileChangeDel != null)
            {
                GlobalEvents.projectileChangeDel(id, transform.position, Vector2.zero, (int)type);
            }

            if (type == BulletDespawnType.SMALL_HIT)
            {
                var explosion = Instantiate(_smallExplosionPrefab, transform.position, _smallExplosionPrefab.transform.rotation);
                Destroy(explosion, 1f);
            }
            else if (type == BulletDespawnType.BIG_HIT)
            {
                // TODO: Implement big boom boom.
            }


            Destroy(this.gameObject);
        }
    }
}
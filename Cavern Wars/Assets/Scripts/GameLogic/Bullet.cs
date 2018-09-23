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

        private Vector2 _direction;

        private float _spawnTime;

        private Rigidbody2D _rigidBody;

        public int Id { get; set; }

        // Use this for initialization
        void Start()
        {
            _spawnTime = Time.time;
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _spawnTime > _timeToLive)
            {
                DespawnBullet(BulletDespawnType.SILENT);
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.otherCollider.CompareTag("Enemy"))
            {
                GlobalEvents.projectileHitDel(collision.otherCollider.GetComponent<EnemyState>().Name, _damage);
            }
        }

        private void DespawnBullet(BulletDespawnType type)
        {
            if (type == BulletDespawnType.NONE)
            {
                return;
            }

            if (GlobalEvents.projectileChangeDel != null)
            {
                GlobalEvents.projectileChangeDel(Id, transform.position, _rigidBody.velocity, (int)type);
            }

            if (type == BulletDespawnType.SMALL_HIT)
            {
                // TODO: Implement small boom boom.
            }
            else if (type == BulletDespawnType.BIG_HIT)
            {
                // TODO: Implement big boom boom.
            }


            Destroy(this.gameObject);
        }
    }
}
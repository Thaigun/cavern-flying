using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    class MoveWithNetwork : MonoBehaviour
    {
        [SerializeField]
        private Engine _engine;

        [SerializeField]
        private NetworkBullet _networkBulletPrefab;

        private List<NetworkBullet> _bullets;

        public Player NetworkPlayer { get; set; }

        void Update()
        {

        }

        private void Start()
        {
            _bullets = new List<NetworkBullet>();
        }

        /// <summary>
        /// Tells the component a new state of the object, received from network. The
        /// component then takes care of moving the game object accordingly.
        /// </summary>
        public void ApplyNewState(GameUpdateMessage msg)
        {
            // TODO: Interpolate or extrapolate to make the movement smooth

            gameObject.SetActive(msg.alive);

            transform.position = msg.position;
            transform.rotation = msg.rotation;

            _engine.SetEnginesActive(msg.enginesOn);

            foreach (ProjectileChange bulletInfo in msg.projectileChanges)
            {
                NetworkBullet netBullet = _bullets.Find(netBul => netBul.Id == bulletInfo.id);
                if (netBullet == null)
                {
                    netBullet = Instantiate(_networkBulletPrefab, transform);
                }
                netBullet.RigidbodyComponent.position = bulletInfo.position;
                netBullet.RigidbodyComponent.velocity = bulletInfo.movement;

                if ((BulletDespawnType)bulletInfo.despawnType != BulletDespawnType.NONE)
                {
                    netBullet.Despawn((BulletDespawnType)bulletInfo.despawnType);
                }
            }
        }
    }
}
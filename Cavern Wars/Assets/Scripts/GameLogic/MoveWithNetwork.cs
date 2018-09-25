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

        [SerializeField]
        private Transform _rotateTransform;

        private List<NetworkBullet> _bullets;
        
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

            transform.position = msg.position;
            _rotateTransform.rotation = msg.rotation;

            _engine.SetEnginesActive(msg.enginesOn);

            foreach (ProjectileChange bulletInfo in msg.projectileChanges)
            {
                NetworkBullet netBullet = _bullets.Find(netBul => netBul.Id == bulletInfo.id);
                if (netBullet == null)
                {
                    netBullet = Instantiate(_networkBulletPrefab, bulletInfo.position, Quaternion.identity);
                    _bullets.Add(netBullet);
                }
                netBullet.Id = bulletInfo.id;
                netBullet.RigidbodyComponent.velocity = bulletInfo.movement;                

                if ((BulletDespawnType)bulletInfo.despawnType != BulletDespawnType.NONE)
                {
                    _bullets.Remove(netBullet);
                    netBullet.Despawn((BulletDespawnType)bulletInfo.despawnType);
                }
            }
        }
    }
}
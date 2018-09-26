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

        [SerializeField]
        private int _bufferSize = 2;

        private List<NetworkBullet> _bullets;

        // When were the interpolation points received
        private Queue<float> _bufferReceiveTimes;
        private Queue<Vector3> _positionBuffer;
        private Queue<Quaternion> _rotationBuffer;
        private bool _interPolationInProgress;

        private void Start()
        {
            _bullets = new List<NetworkBullet>();
            _bufferReceiveTimes = new Queue<float>();
            _positionBuffer = new Queue<Vector3>();
            _rotationBuffer = new Queue<Quaternion>();
        }

        void Update()
        {
            // TODO: Interpolate to make the movement smooth
            /*if (!_interPolationInProgress && _bufferReceiveTimes.Count >= _bufferSize)
            {
                StartCoroutine(InterpolateMovement());
            }*/
            if (_bufferReceiveTimes.Count > 0)
            {
                _bufferReceiveTimes.Dequeue();
                transform.position = _positionBuffer.Dequeue();
                _rotateTransform.rotation = _rotationBuffer.Dequeue();
            }
        }

        private IEnumerator InterpolateMovement()
        {
            _interPolationInProgress = true;

            float startTime, endTime, timePortion;
            Vector3 startPos, endPos;
            Quaternion startRot, endRot;

            do
            {
                startTime = _bufferReceiveTimes.Dequeue();
                startPos = _positionBuffer.Dequeue();
                startRot = _rotationBuffer.Dequeue();

                endTime = _bufferReceiveTimes.Peek();
                endPos = _positionBuffer.Peek();
                endRot = _rotationBuffer.Peek();

                timePortion = (Time.time - startTime) / (endTime - startTime);

                _rotateTransform.rotation = Quaternion.Lerp(startRot, endRot, timePortion);
                transform.position = Vector3.Lerp(startPos, endPos, timePortion);

                yield return null;
            } while (timePortion < 1);

            _interPolationInProgress = false;
        }

        /// <summary>
        /// Tells the component a new state of the object, received from network. The
        /// component then takes care of moving the game object accordingly.
        /// </summary>
        public void ApplyNewState(GameUpdateMessage msg)
        {
            _positionBuffer.Enqueue(msg.position);
            _rotationBuffer.Enqueue(msg.rotation);
            _bufferReceiveTimes.Enqueue(Time.time);
            
            // Engines and bullets can be switched on and off immediately.
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
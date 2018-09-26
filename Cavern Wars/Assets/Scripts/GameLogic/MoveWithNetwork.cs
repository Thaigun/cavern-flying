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
        private bool _justWokeUp;

        // When were the interpolation points received
        private Queue<float> _bufferReceiveTimes;
        private Queue<Vector3> _positionBuffer;
        private Queue<Quaternion> _rotationBuffer;
        private bool _interPolationInProgress;

        private void Awake()
        {
            _bullets = new List<NetworkBullet>();
            _bufferReceiveTimes = new Queue<float>();
            _positionBuffer = new Queue<Vector3>();
            _rotationBuffer = new Queue<Quaternion>();

            _justWokeUp = true;
        }

        void Update()
        {
            if (_bufferReceiveTimes.Count >= 2 && !_interPolationInProgress)
            {
                if (_justWokeUp)
                {
                    _justWokeUp = false;
                    transform.position = _positionBuffer.Peek();
                    _rotateTransform.rotation = _rotationBuffer.Peek();
                }
                else
                {
                    StartCoroutine(InterpolateMovement());
                }
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
                _positionBuffer.Dequeue();
                _rotationBuffer.Dequeue();
                startPos = transform.position;
                startRot = _rotateTransform.rotation;

                endTime = _bufferReceiveTimes.Peek();
                endPos = _positionBuffer.Peek();
                endRot = _rotationBuffer.Peek();

                timePortion = (Time.time - startTime) / (endTime - startTime);

                _rotateTransform.rotation = Quaternion.LerpUnclamped(startRot, endRot, timePortion);
                transform.position = Vector3.LerpUnclamped(startPos, endPos, timePortion);

                yield return null;
            } while (_bufferReceiveTimes.Count < 2 && timePortion < 5);

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
                    AudioManager.Instance.PlayClip(AudioManager.Instance.shot, false);
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
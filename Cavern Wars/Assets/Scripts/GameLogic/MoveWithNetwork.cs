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
        private float _previousReceiveTime;
        private float _latestReceiveTime;
        private Vector3 _latestPosition;
        private Quaternion _latestRotation;

        Coroutine _interpolationCoroutine;

        private void Awake()
        {
            _bullets = new List<NetworkBullet>();
        }

        private void OnEnable()
        {
            _previousReceiveTime = 0f;
            _latestReceiveTime = -1f;
            _latestPosition = transform.position;
            _latestRotation = _rotateTransform.rotation;

            _justWokeUp = true;
        }

        void Update()
        {
            // This could be implemented by just calling the interpolation coroutine directly from 
            // where the message is received
            if (_latestReceiveTime > _previousReceiveTime)
            {
                if (_justWokeUp)
                {
                    _justWokeUp = false;
                    transform.position = _latestPosition;
                    _rotateTransform.rotation = _latestRotation;
                }
                else
                {
                    if (_interpolationCoroutine != null)
                    {
                        StopCoroutine(_interpolationCoroutine);
                    }
                    _interpolationCoroutine = StartCoroutine(InterpolateMovement());
                }

                _previousReceiveTime = _latestReceiveTime + 0.0001f;
            }
        }

        private IEnumerator InterpolateMovement()
        {
            float startTime = Time.time;
            float endTime = Time.time + (_latestReceiveTime - _previousReceiveTime);
            float timePortion = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = _latestPosition;
            Quaternion startRot = _rotateTransform.rotation;
            Quaternion endRot = _latestRotation;

            Debug.Log("prev and latest: " + _previousReceiveTime + ", " + _latestReceiveTime);

            do
            {
                timePortion = (Time.time - startTime) / (endTime - startTime);
                Debug.Log("TimePortion: " + timePortion);

                _rotateTransform.rotation = Quaternion.LerpUnclamped(startRot, endRot, timePortion);
                transform.position = Vector3.LerpUnclamped(startPos, endPos, timePortion);

                yield return null;
            } while (timePortion < 20);
            
        }

        /// <summary>
        /// Tells the component a new state of the object, received from network. The
        /// component then takes care of moving the game object accordingly.
        /// </summary>
        public void ApplyNewState(GameUpdateMessage msg)
        {
            _latestPosition = msg.position;
            _latestRotation = msg.rotation;
            _latestReceiveTime = Time.time;
            
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
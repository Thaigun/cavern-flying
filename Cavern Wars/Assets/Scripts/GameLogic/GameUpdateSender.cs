using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars {
    /// <summary>
    /// Sends the locally simulated game state to other clients / host.
    /// </summary>
    public class GameUpdateSender : MonoBehaviour
    {
        [SerializeField]
        private float _packetsPerSecond = 15f;

        [SerializeField]
        private Transform _playerTransform;

        [SerializeField]
        private MoveWithWASD _moveWithWASD;

        [SerializeField]
        private ShootWithMouse _shooter;

        private float _sendInterval;
        private float _lastPacketTime;

        private List<ProjectileChange> _projectilesForNextMessage;
        private List<Hit> _hitsForNextMessage;

        // Use this for initialization
        void Start()
        {
            _sendInterval = 1f / _packetsPerSecond;
            _lastPacketTime = Time.time;
            _projectilesForNextMessage = new List<ProjectileChange>();
            _hitsForNextMessage = new List<Hit>();

            GlobalEvents.projectileChangeDel += OnProjectileChange;
            GlobalEvents.projectileHitDel += OnProjectileHit;
        }

        // Update is called once per frame
        void Update()
        {
            if (NetworkInterface.Instance == null)
            {
                return;
            }
            if (Time.time - _lastPacketTime > _sendInterval)
            {
                _lastPacketTime = Time.time;
                SendCurrentState();
            }
        }

        private void SendCurrentState()
        {
            GameUpdateMessage updateMessage = new GameUpdateMessage();
            updateMessage.position = _playerTransform.position;
            updateMessage.rotation = _playerTransform.rotation;
            updateMessage.enginesOn = _moveWithWASD.EnginesOn;
            updateMessage.alive = GameController.Instance.WantsToLive;
            updateMessage.projectileChanges = _projectilesForNextMessage.ToArray();
            _projectilesForNextMessage.Clear();
            NetworkInterface.Instance.SendToAllConnected(MessageType.GAME_UPDATE, updateMessage, NetworkInterface.Instance.UnreliableChannel);

            PlayerHitMessage hitMessage = new PlayerHitMessage();
            hitMessage.damages = new float[_hitsForNextMessage.Count];
            hitMessage.hitPlayers = new string[_hitsForNextMessage.Count];
            for (int i = 0; i < _hitsForNextMessage.Count; i++)
            {
                Hit hit = _hitsForNextMessage[i];
                hitMessage.damages[i] = hit.damage;
                hitMessage.hitPlayers[i] = hit.targetName;
            }
            _hitsForNextMessage.Clear();
            NetworkInterface.Instance.Send(MessageType.HIT_MESSAGE, hitMessage, PartyManager.Instance.HostConnectionId, NetworkInterface.Instance.ReliableChannel);
        }

        private void OnProjectileChange(int idArg, Vector2 positionArg, Vector2 movementArg, int despawnTypeArg)
        {
            ProjectileChange newChange = new ProjectileChange()
            {
                id = idArg,
                position = positionArg,
                movement = movementArg,
                despawnType = despawnTypeArg
            };
            _projectilesForNextMessage.Add(newChange);
        }

        private void OnProjectileHit(string targetNameArg, float damageArg)
        {
            _hitsForNextMessage.Add(new Hit()
            {
                targetName = targetNameArg, 
                damage = damageArg
            });
        }
    }

    struct Hit
    {
        public string targetName;
        public float damage;
    }
}
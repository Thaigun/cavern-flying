using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    class PlayerState : MonoBehaviour
    {
        [SerializeField]
        private HealthBar _healthBar;

        private MoveWithNetwork _moveWithNetwork;

        public Player NetworkPlayer { get; set; }

        public HealthBar HPBar { get { return _healthBar; } }

        public MoveWithNetwork MoveWithNetworkComponent
        {
            get
            {
                if (_moveWithNetwork == null)
                {
                    _moveWithNetwork = GetComponent<MoveWithNetwork>();
                }
                return _moveWithNetwork;
            }
        }
        
        public void ApplyNewState(GameUpdateMessage msg)
        {
            if (MoveWithNetworkComponent)
            {
                MoveWithNetworkComponent.ApplyNewState(msg);
            }
        }

        public void UpdateColor()
        {
            GetComponentInChildren<MaterialChanger>().UpdateColor(this.NetworkPlayer.Name);
        }

        /// <summary>
        /// Despawn (kill) the enemy. Do nothing if already dead.
        /// </summary>
        public void Despawn()
        {
            // TODO: Boom boom
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Spawn the enemy. Do nothing if already alive.
        /// </summary>
        public void Spawn()
        {
            gameObject.SetActive(true);
        }
    }
}
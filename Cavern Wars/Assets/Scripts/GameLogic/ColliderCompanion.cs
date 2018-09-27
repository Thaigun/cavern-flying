using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    /// <summary>
    /// Player ship had to be rotated around x-axis, which meant that 2D collider couldn't be attached to it
    /// because of how Unity works. But the collider has to rotate with the ship, so it is in a child object.
    /// This class is meant to be a companion for the colliser and provides access to the parent components.
    /// </summary>
    class ColliderCompanion : MonoBehaviour
    {
        [SerializeField]
        PlayerState _ownerPlayer;

        public PlayerState OwnerPlayer { get { return _ownerPlayer; } }
    }
}
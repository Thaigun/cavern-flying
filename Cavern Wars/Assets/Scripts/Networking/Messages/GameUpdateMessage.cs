using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using CavernWars;


namespace CavernWars
{
    /// <summary>
    /// Message sent by a player to update their state to other players.
    /// </summary>
    public class GameUpdateMessage : MessageBase
    {
        public Vector2 position;
        public Quaternion rotation;
        public bool enginesOn;
        public bool alive;
        public ProjectileChange[] projectileChanges;
    }

    // TODO: Check if the Serializable tag / inheriting MessageBase are needed.
    [Serializable]
    public class ProjectileChange : MessageBase
    {
        public int id;
        public Vector2 position;
        public Vector2 movement;
        public int despawnType; // None, silent, smallHit, bigHit
    }
}
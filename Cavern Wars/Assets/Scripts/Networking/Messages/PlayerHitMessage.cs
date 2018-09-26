using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// This should be sent through a reliable channel. 
    /// It's sent to the host by the client whenever there is a new hit.
    /// </summary>
    public class PlayerHitMessage : MessageBase
    {
        public string[] hitPlayers;
        public float[] damages;
    }
}

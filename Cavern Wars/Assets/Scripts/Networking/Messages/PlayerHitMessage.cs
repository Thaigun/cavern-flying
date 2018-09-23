using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    public class PlayerHitMessage : MessageBase
    {
        public string[] hitPlayers;
        public float[] damages;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    public class PlayersHealthMessage : MessageBase
    {
        public string[] playerNames;
        public float[] healths;
        public float maxHealth;
    }
}

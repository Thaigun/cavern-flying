using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    class ScoreMessage : MessageBase
    {
        public string[] playerNames;
        public int[] deaths;
        public int[] kills;
    }
}

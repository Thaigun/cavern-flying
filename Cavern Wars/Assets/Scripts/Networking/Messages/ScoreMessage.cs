using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// The host sends this to all players whenever there are changes.
    /// </summary>
    class ScoreMessage : MessageBase
    {
        public string[] playerNames;
        public int[] deaths;
        public int[] kills;
    }
}

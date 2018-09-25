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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < playerNames.Length; i++)
            {
                sb.Append(playerNames[i] + ": " + healths[i].ToString("F1") + ", ");
            }
            return sb.ToString();
        }
    }
}

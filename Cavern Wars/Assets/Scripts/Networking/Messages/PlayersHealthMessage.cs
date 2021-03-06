﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    ///  The host keeps track of the health of all players. This is sent to all clients.
    /// </summary>
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

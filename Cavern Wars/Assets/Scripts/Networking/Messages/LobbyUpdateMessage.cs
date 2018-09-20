using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// Public members of the class derived from MessageBase will be serialized and deserialized
    /// </summary>
    public class LobbyUpdateMessage : MessageBase
    {
        public int map;
        public LobbyPlayerInfo[] players;
    }

    [Serializable]
    public class LobbyPlayerInfo : MessageBase
    {
        public string name;
        public int id;
        public string ip;
        public bool host;
    }
}
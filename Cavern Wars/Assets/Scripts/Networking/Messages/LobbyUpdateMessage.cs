using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// The host updates the state of the lobby to joined players once a second.
    /// </summary>
    public class LobbyUpdateMessage : MessageBase
    {
        public int map; // -1 means lobby/not ready, 1, 2, 3... mean the according map.
        public LobbyPlayerInfo[] players;
    }

    [Serializable]
    public class LobbyPlayerInfo : MessageBase
    {
        public string name;
        public string ip;
        public int port;
        public bool isHost;
   
        public LobbyPlayerInfo()
        {

        }

        public LobbyPlayerInfo(Player plr)
        {
            name = plr.Name;
            ip = plr.Ip;
            port = plr.Port;
            isHost = plr.IsHost;
        }
    }
}
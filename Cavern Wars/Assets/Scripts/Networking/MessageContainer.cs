using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    public class MessageContainer
    {
        public MessageType MsgType { get; set; }
        public MessageBase Message { get; set; }
        public int HostId { get; set; }
        public int ConnectionId { get; set; }
    }
}

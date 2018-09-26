using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// Player sends their information to the host
    /// </summary>
    public class PlayerInfoMessage : MessageBase
    {
        public string name;
    }
}

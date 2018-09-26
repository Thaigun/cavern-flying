using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// // Host sends this to all other clients in order to start the match
    /// </summary>
    public class MatchStatusMessage : MessageBase
    {
        public int matchStatus;
    }
}

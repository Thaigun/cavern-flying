using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CavernWars
{
    /// <summary>
    /// Tells to the host that the client is ready to start the match
    /// </summary>
    public class SceneLoadedMessage : MessageBase
    {
        public int sceneLoaded;
    }
}

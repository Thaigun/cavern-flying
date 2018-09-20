using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars {
    public class Player
    {
        public string Name { get; set; }

        public string Ip { get; private set; }

        public int Port { get; private set; }

        public int ConnectionId { get; private set; }

        public bool IsYou { get; private set; }

        public bool IsHost { get; private set; }

        public bool Ready { get; set; }

        public void SetData(string name, string ip, int port, int connectionId, bool isHost, bool isYou)
        {
            Name = name;
            Ip = ip;
            Port = port;
            ConnectionId = connectionId;
            IsHost = isHost;
            IsYou = isYou;
        }

        public Player(string name, string ip, int port, int connectionId, bool IsHost, bool isYou)
        {
            SetData(name, ip, port, connectionId, IsHost, isYou);
        }
    }
}
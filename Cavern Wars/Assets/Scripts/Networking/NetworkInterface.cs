using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Net;
using System.Net.Sockets;

namespace CavernWars
{
    // Network interface polls the low level system for received network messages.
    // Other classes can register to handle those events.
    public delegate void ConnectionResponseDel(int connectionId);
    public delegate void DisconnectDel(int connectionId);
    public delegate void DataDel(MessageContainer msgContainer);

    public class NetworkInterface : MonoBehaviour
    {
        [SerializeField]
        private int _maxConnections = 5;

        [SerializeField]
        private bool _logAll;

        private int _hostId;
        private bool _networkTransportStarted;

        public static NetworkInterface Instance { get; private set; }

        // These delagates will be called whenever a message is received.
        public ConnectionResponseDel connectionResponseDel;
        public DisconnectDel disconnectDel;
        public DataDel gameUpdateDel;
        public DataDel lobbyUpdateDel;
        public DataDel matchStatusDel;
        public DataDel playerInfoDel;
        public DataDel sceneLoadedDel;
        public DataDel playerHitDel;
        public DataDel playerHealthDel;
        public DataDel scoreDel;

        public HashSet<int> WaitingConnectionIds { get; private set; }
        public HashSet<int> ConnectionIds { get; private set; }
        public byte ReliableChannel { get; private set; }
        public byte UnreliableChannel { get; private set; }

        // Use this for initialization
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            ConnectionIds = new HashSet<int>();
            WaitingConnectionIds = new HashSet<int>();
        }
        
        void Update()
        {
            if (_networkTransportStarted)
            {
                ReceiveMessages();
            }
        }

        private void ReceiveMessages()
        {
            int recHostId;
            int recConnectionId;
            int recChannelId;
            int recSize;
            byte[] buffer;
            byte error;
            NetworkError err;

            NetworkEventType evtType = NetworkEventType.Nothing;

            do
            {
                int bufferSize = 500;
                // If the message does not fit, double the buffer size.
                do
                {
                    buffer = new byte[bufferSize];
                    evtType = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, buffer, bufferSize, out recSize, out error);
                    err = (NetworkError)error;
                    bufferSize *= 2;
                } while (err == NetworkError.MessageToLong);

                switch (evtType)
                {
                    case NetworkEventType.DataEvent:
                        OnData(buffer, recHostId, recConnectionId);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        OnDisconnect(recConnectionId);
                        break;
                    case NetworkEventType.ConnectEvent:
                        OnConnect(recConnectionId);
                        break;
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
            } while (evtType != NetworkEventType.Nothing);
        }

        private void OnData(byte[] buffer, int hostId, int connectionId)
        {
            MessageContainer msgContainer = ReadData(buffer);
            // Populate rest of the message container.
            msgContainer.ConnectionId = connectionId;
            msgContainer.HostId = hostId;
            
            // Call registered delegate methdos for different message types.
            switch (msgContainer.MsgType)
            {
                case (MessageType.GAME_UPDATE):
                    if (gameUpdateDel != null)
                    {
                        gameUpdateDel(msgContainer);
                    }
                    break;
                case (MessageType.LOBBY_UPDATE):
                    if (lobbyUpdateDel != null)
                    {
                        lobbyUpdateDel(msgContainer);
                    }
                    break;
                case (MessageType.MATCH_STATUS):
                    if (matchStatusDel != null)
                    {
                        matchStatusDel(msgContainer);
                    }
                    break;
                case (MessageType.PLAYER_INFO):
                    if (playerInfoDel != null)
                    {
                        playerInfoDel(msgContainer);
                    }
                    break;
                case (MessageType.SCENE_LOADED):
                    if (sceneLoadedDel != null)
                    {
                        sceneLoadedDel(msgContainer);
                    }
                    break;
                case (MessageType.HIT_MESSAGE):
                    if (playerHitDel != null)
                    {
                        playerHitDel(msgContainer);
                    }
                    break;
                case (MessageType.HEALTH_MESSAGE):
                    if (playerHealthDel != null)
                    {
                        playerHealthDel(msgContainer);
                    }
                    break;
                case (MessageType.SCORE_MESSAGE):
                    if (scoreDel != null)
                    {
                        scoreDel(msgContainer);
                    }
                    break;
            }
        }

        private void OnConnect(int connectionId)
        {
            // In case of a client, who makes the initial connection and receives response for it.
            if (WaitingConnectionIds.Contains(connectionId))
            {
                if (connectionResponseDel != null)
                {
                    connectionResponseDel(connectionId);
                }
                WaitingConnectionIds.Remove(connectionId);
            }

            ConnectionIds.Add(connectionId);
        }

        private void OnDisconnect(int connectionId)
        {
            ConnectionIds.Remove(connectionId);
            if (this.disconnectDel != null)
            {
                disconnectDel(connectionId);
            }
        }

        /// <summary>
        /// Reads the data from inside the buffer and creates a message container that
        /// allows easy access to the message. Populates the container with the message
        /// content and the message type.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private MessageContainer ReadData(byte[] buffer)
        {
            MessageContainer container = new MessageContainer();
            NetworkReader reader = new NetworkReader(buffer);
            var messageSizeData = reader.ReadBytes(2); // Size of the message
            byte[] messageTypeData = reader.ReadBytes(2); // Type of the message
            // Convert the byte array to short.
            short messageType = (short)((messageTypeData[1] << 8) + messageTypeData[0]);
            MessageType msgType = (MessageType)messageType;
            container.MsgType = msgType;
            //reader.SeekZero();

            MessageBase message;
            switch (msgType)
            {
                case MessageType.GAME_UPDATE:
                    message = new GameUpdateMessage();
                    break;
                case MessageType.LOBBY_UPDATE:
                    message = new LobbyUpdateMessage();
                    break;
                case MessageType.MATCH_STATUS:
                    message = new MatchStatusMessage();
                    break;
                case MessageType.SCENE_LOADED:
                    message = new SceneLoadedMessage();
                    break;
                case MessageType.PLAYER_INFO:
                    message = new PlayerInfoMessage();
                    break;
                case MessageType.HIT_MESSAGE:
                    message = new PlayerHitMessage();
                    break;
                case MessageType.HEALTH_MESSAGE:
                    message = new PlayersHealthMessage();
                    break;
                case MessageType.SCORE_MESSAGE:
                    message = new ScoreMessage();
                    break;
                default:
                    throw new System.Exception("Message type was unexpected: " + msgType);
            }
            message.Deserialize(reader);
            container.Message = message;
            return container;
        }

        private void InitNetworkTransport()
        {
            NetworkTransport.Init();
            _networkTransportStarted = true;
        }

        private void ShutdownNetworkTransport()
        {
            NetworkTransport.Shutdown();
            _networkTransportStarted = false;
        }

        public void OpenSocket()
        {
            InitNetworkTransport();
            ConnectionConfig config = new ConnectionConfig();
            ReliableChannel = config.AddChannel(QosType.Reliable);
            UnreliableChannel = config.AddChannel(QosType.UnreliableSequenced);
            HostTopology topology = new HostTopology(config, _maxConnections);
            _hostId = NetworkTransport.AddHost(topology);
        }

        public int ConnectToIP(string ip, int port)
        {
            
            byte error;
            NetworkError err;
            int connectionId;
            if (ip.Equals("local"))
            {
                StartCoroutine(DelayedConnectionResponse(-1));
                connectionId = -1;
                err = NetworkError.Ok;
            }
            else
            {
                connectionId = NetworkTransport.Connect(_hostId, ip, port, 0, out error);
                err = (NetworkError)error;
            }

            if (err == NetworkError.Ok)
            {
                WaitingConnectionIds.Add(connectionId);
            }
            return connectionId;
        }

        /// <summary>
        /// If the connection is made to self, then we want to imitate the connection response 
        /// on the next tick to allow the client to act as if it was coming from a remote host.
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        private IEnumerator DelayedConnectionResponse(int connId)
        {
            yield return null;
            if (this.connectionResponseDel != null)
            {
                // First receive the connection
                OnConnect(connId);
                // Then respond to it
                this.connectionResponseDel(connId);
            }
        }

        public void SendToAllConnected(MessageType msgType, MessageBase msg, int channelId = -1, bool excludeSelf = false)
        {
            if (channelId == -1) { channelId = ReliableChannel; }
            foreach (int connId in ConnectionIds)
            {
                if (connId == -1 && excludeSelf)
                {
                    continue;
                }
                Send(msgType, msg, connId, channelId);
            }
        }

        public void Send(MessageType msgType, MessageBase msg, int connectionId, int channelId = -1)
        {
            if (channelId == -1) { channelId = ReliableChannel; }
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage((short)msgType);
            msg.Serialize(writer);
            writer.FinishMessage();
            byte[] byteMsg = writer.ToArray();
            byte error;
            // If sending to self
            if (connectionId == -1)
            {
                OnData(byteMsg, _hostId, -1);
            }
            else
            {
                NetworkTransport.Send(_hostId, connectionId, channelId, byteMsg, byteMsg.Length, out error);
                NetworkError err = (NetworkError)error;
            }
        }

        public void CloseConnections()
        {
            foreach (var connId in ConnectionIds)
            {
                byte error;
                NetworkTransport.Disconnect(_hostId, connId, out error);
                NetworkError err = (NetworkError)error;
            }
            ConnectionIds.Clear();
            WaitingConnectionIds.Clear();
            ShutdownNetworkTransport();
        }

        /// <summary>
        /// Get a string representing your IP address and port currently used by NetworkTransport.
        /// This only recognizes the IPv6 address at the moment and returns the first one that it finds.
        /// </summary>
        /// <returns></returns>
        public List<IPAddress> GetYourIp(out int port)
        {
            List<IPAddress> possibleAddresses = new List<IPAddress>();
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            for (int i = 0; i < addresses.Length; i++)
            {
                IPAddress deviceIP = addresses[i];
                if (deviceIP.AddressFamily == AddressFamily.InterNetworkV6 || deviceIP.AddressFamily == AddressFamily.InterNetwork)
                {
                    possibleAddresses.Add(deviceIP);
                }
            }
            port = NetworkTransport.GetHostPort(_hostId);
            return possibleAddresses;
        }

        /// <summary>
        /// Gets the ip address for a connection identified by its connection id.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="port">The open port for the connection is populated here</param>
        /// <returns>IP-address of the connection</returns>
        public string GetConnectionIp(int connectionId, out int port)
        {
            if (connectionId == -1)
            {
                port = -1;
                return "local";
            }

            NetworkID networkId;
            NodeID nodeId;
            byte error;
            string address;

            NetworkTransport.GetConnectionInfo(_hostId, connectionId, out address, out port, out networkId, out nodeId, out error);

            var err = (NetworkError)error;
            if (err != NetworkError.Ok)
            {
                throw new System.Exception("Error with getting connection info: " + err);
            }
            return address;
        }
    }
}

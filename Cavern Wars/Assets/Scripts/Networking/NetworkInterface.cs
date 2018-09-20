using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CavernWars
{
    public class NetworkInterface : MonoBehaviour
    {
        [SerializeField]
        private int _maxConnections = 5;

        private int _hostId;
        private HashSet<int> _waitingConnectionIds; // Connections that have not been confirmed by the other party.
        private HashSet<int> _connectionIds; // Confirmed connections.
        private bool _networkTransportStarted;

        public static NetworkInterface Instance { get; private set; }
        public Queue<MessageBase> ReceivedMessages { get; private set; }

        // Use this for initialization
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _connectionIds = new HashSet<int>();
            _waitingConnectionIds = new HashSet<int>();
            ReceivedMessages = new Queue<MessageBase>();
        }

        // Update is called once per frame
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
            byte[] buffer = new byte[20000];
            byte error;

            NetworkEventType evtType = NetworkEventType.Nothing;

            do
            {
                evtType = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, buffer, buffer.Length, out recSize, out error);
                if (recSize > buffer.Length)
                {
                    // TODO: Solve this.
                    Debug.LogWarning("Received message longer than the buffer. Expect problems");
                }

                switch (evtType)
                {
                    case NetworkEventType.DataEvent:
                        OnData(buffer);
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

        private void OnData(byte[] buffer)
        {
            ReceivedMessages.Enqueue(ReadData(buffer));
            Debug.Log("Received data message");
        }

        private void OnConnect(int connectionId)
        {
            // In case of a client, who makes the initial connection
            if (_waitingConnectionIds.Contains(connectionId))
            {
                Debug.Log("Connection response received");
                _waitingConnectionIds.Remove(connectionId);
            }
            // In case of a host, who receives connections
            else
            {
                Debug.Log("Connection received: " + connectionId);
            }
            _connectionIds.Add(connectionId);
        }

        private void OnDisconnect(int connectionId)
        {
            _connectionIds.Remove(connectionId);
            Debug.Log("Connection sidconnected: " + connectionId);
        }

        private MessageBase ReadData(byte[] buffer)
        {
            NetworkReader reader = new NetworkReader(buffer);
            var messageSizeData = reader.ReadBytes(2); // Size of the message
            byte[] messageTypeData = reader.ReadBytes(2); // Type of the message
            // Convert the byte array to short.
            short messageType = (short)((messageTypeData[1] << 8) + messageTypeData[0]);
            MessageType msgType = (MessageType)messageType;
            reader.SeekZero();

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
                default:
                    throw new System.Exception("Message type was unexpected: " + msgType);
            }
            message.Deserialize(reader);
            return message;
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
            HostTopology topology = new HostTopology(config, _maxConnections);
            _hostId = NetworkTransport.AddHost(topology);
        }

        public void ConnectToIP(string ip, int port)
        {
            byte error;
            int connectionId = NetworkTransport.Connect(_hostId, ip, port, 0, out error);
            NetworkError err = (NetworkError)error;
            if (err == NetworkError.Ok)
            {
                _waitingConnectionIds.Add(connectionId);
            }
        }

        public void SendToAllConnected(MessageType msgType, MessageBase msg, int channelId = 0)
        {
            foreach (int connId in _connectionIds)
            {
                Send(msgType, msg, channelId, connId);
            }
        }

        public void Send(MessageType msgType, MessageBase msg, int channelId, int connectionId)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage((short)msgType);
            msg.Serialize(writer);
            writer.FinishMessage();
            byte[] byteMsg = writer.ToArray();
            byte error;
            NetworkTransport.Send(_hostId, connectionId, channelId, byteMsg, byteMsg.Length, out error);
            NetworkError err = (NetworkError)error;
        }

        public void CloseConnections()
        {
            foreach (var connId in _connectionIds)
            {
                byte error;
                NetworkTransport.Disconnect(_hostId, connId, out error);
                NetworkError err = (NetworkError)error;
            }
            _connectionIds.Clear();
            _waitingConnectionIds.Clear();
            ShutdownNetworkTransport();
        }

        public string GetIP()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            for (int i = 0; i < addresses.Length; i++)
            {
                IPAddress deviceIP = addresses[i];
                if (deviceIP.AddressFamily == AddressFamily.InterNetwork)
                {
                    int port = NetworkTransport.GetHostPort(_hostId);
                    return deviceIP.ToString() + ":" + port;
                }
            }
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CavernWars;

public class NetworkInterface : MonoBehaviour
{
    [SerializeField]
    private int _port = 3008;

    [SerializeField]
    private int _maxConnections = 5;

    private int _hostId;
    private HashSet<int> _connectionIds;

    public static NetworkInterface Instance { get; private set; }
    public Queue<MessageBase> ReceivedMessages { get; private set; }

    // Use this for initialization
    void Start ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _connectionIds = new HashSet<int>();
        ReceivedMessages = new Queue<MessageBase>();
	}
	
	// Update is called once per frame
	void Update ()
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

            switch (evtType)
            {
                case NetworkEventType.DataEvent:
                    ReceivedMessages.Enqueue(ReadData(buffer));
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.LogWarning("DisconnectEvent not implemented");
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.LogWarning("Connect event not implemented");
                    break;
                case NetworkEventType.Nothing:
                    Debug.LogWarning("Nothing event not implemented");
                    break;
                case NetworkEventType.BroadcastEvent:
                    Debug.LogWarning("Broadcast event not implemented");
                    break;
            }
        } while (evtType != NetworkEventType.Nothing);

             
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
                message = new SingleIntMessage();
                break;
            case MessageType.SCENE_LOADED:
                message = new SingleIntMessage();
                break;
            default:
                throw new System.Exception("Message type was unexpected: " + msgType);
        }
        message.Deserialize(reader);
        return message;
    }

    public void OpenSocket()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        HostTopology topology = new HostTopology(config, _maxConnections);
        _hostId = NetworkTransport.AddHost(topology, _port);
    }

    public void ConnectToIP(string ip)
    {
        byte error;
        int connectionId = NetworkTransport.Connect(_hostId, ip, _port, 0, out error);
        NetworkError err = (NetworkError)error;
        _connectionIds.Add(connectionId);
    }

    public void Send(short msgType, MessageBase msg, int channelId, int connectionId)
    {
        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(msgType);
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
        NetworkTransport.Shutdown();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInterface : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        NetworkTransport.Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void PrepareForCnnections()
    {

    }

    public void ConnectToIP(string ip, int port)
    {

    }

    public void Send()
    {

    }

    public void CloseConnections()
    {

    }
}

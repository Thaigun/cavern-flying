using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyPlayer : MonoBehaviour
{
    public string Name
    {
        get
        {
            return GetComponent<Text>().text;
        }
        set
        {
            GetComponent<Text>().text = value;
        }
    }

    public string Ip { get; private set; }

    public int Id { get; private set; }

    public bool IsYou { get; private set; }

	public void SetData(string name, string ip, int id, bool isYou)
    {
        Name = name;
        Ip = ip;
        Id = id;
        IsYou = isYou;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undestroyable : MonoBehaviour
{
    private static Undestroyable _instance;

	// Use this for initialization
	void Awake ()
    {
        if (_instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
	}

}

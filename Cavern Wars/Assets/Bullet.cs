using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 _direction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Shoots the bullet towards target
    /// </summary>
    /// <param name="direction">Represents direction and speed of the bullet</param>
    public void ShootTo(Vector3 direction)
    {
        Debug.Log("Shoot to: " + direction);
    }
}

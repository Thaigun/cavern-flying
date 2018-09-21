using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithWASD : MonoBehaviour
{
    [SerializeField]
    private float _thrustForce = 1f;

    private Rigidbody2D _rigidBody;

    private float ThrustForce { get { return _thrustForce; } }

	void Start ()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        // To which direction is the player controlling towards
        Vector3 steerDirection = Vector3.zero; 
        if (Input.GetKey(KeyCode.W))
        {
            steerDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            steerDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            steerDirection += Vector3.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            steerDirection += Vector3.right;
        }

        Vector3 accel = steerDirection.normalized * ThrustForce;
        _rigidBody.AddForce(accel);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTowardsMovement : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 10f;

    private Vector3 _previousPosition;
    private Vector3 _directionOfMovement;
    // Rotation that means the spaceship is heading up.
    private Quaternion _referenceUp;

    private void Start()
    {
        _previousPosition = transform.position;
        _directionOfMovement = Vector3.up;
        _referenceUp = transform.rotation;
    }

    void Update ()
    {
        _directionOfMovement = transform.position - _previousPosition;
        if (!Vector3.zero.Equals(_directionOfMovement))
        {
            var lookRot = Quaternion.LookRotation(_directionOfMovement, Vector3.back);
            var rotation = Quaternion.RotateTowards(transform.rotation, lookRot, _rotationSpeed);
            transform.rotation = rotation;
        }
	}

    private void LateUpdate()
    {
        _previousPosition = transform.position;
    }
}

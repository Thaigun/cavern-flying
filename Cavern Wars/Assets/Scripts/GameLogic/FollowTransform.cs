using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    private Transform _followed;

    [SerializeField]
    private bool _freezeX;

    [SerializeField]
    private bool _freezeY;

    [SerializeField]
    private bool _freezeZ = true;

    [SerializeField, Tooltip("How quickly the transform is followed, larger number means more strict following. (0, 1]")]
    private float _followFactor = 0.1f;

    // Update is called once per frame
    void FixedUpdate ()
    {
        _followFactor = Mathf.Clamp01(_followFactor);
        Vector3 translateVector = (_followed.position - transform.position) * _followFactor;
        if (_freezeX)
        {
            translateVector.x = 0;
        }
        if (_freezeY)
        {
            translateVector.y = 0;
        }
        if (_freezeZ)
        {
            translateVector.z = 0;
        }
        transform.Translate(translateVector, Space.World);
	}
}

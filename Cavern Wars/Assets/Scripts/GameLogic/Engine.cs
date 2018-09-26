using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

	public void SetEnginesActive(bool on)
    {
        _particleSystem.gameObject.SetActive(on);
    }
}

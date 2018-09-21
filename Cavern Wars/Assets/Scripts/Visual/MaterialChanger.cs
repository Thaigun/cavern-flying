using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialChanger : MonoBehaviour
{
    [SerializeField]
    private List<Material> _materials;

    private MeshRenderer _meshRenderer;

	// Use this for initialization
	void Start ()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
	}
	
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerZoneComponent : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        Activate(false);
    }
	

    public void Activate(bool active)
    {
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = active;
        }
    }
}

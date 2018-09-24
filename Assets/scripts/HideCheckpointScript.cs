using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*This simple script it used to hide the red spheres that represents checkpoints at runtime. I wanted to be visible in editor and i choose to write this script*/
public class HideCheckpointScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        MeshRenderer meshComponent = gameObject.GetComponent<MeshRenderer>();
        if (meshComponent)
        {
            meshComponent.enabled = false;
        }
	}
}

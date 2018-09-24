using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDeleteComponent : MonoBehaviour {
    private bool isActive = false;

    public bool InDeleteMode
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
            SetColor(Color.white);
        }
    }
    // Use this for initialization

    void Start ()
    {
        InDeleteMode = false;
	}
	
    void OnMouseEnter()
    {
        if (InDeleteMode)
            SetColor(Color.red);
    }

    private void OnMouseExit()
    {
        if (InDeleteMode)
            SetColor(Color.white);
    }

    private void OnMouseOver()
    {
        if (InDeleteMode && Input.GetMouseButtonDown(0))
            Destroy(gameObject);
    }

    private void SetColor(Color c)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = c;
            }
        } 
    }

}

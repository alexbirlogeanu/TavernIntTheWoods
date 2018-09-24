using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTowerScript : MonoBehaviour
{
    private List<GameObject> Towers = new List<GameObject>();
	// Use this for initialization
	void Start ()
    {
        enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(1))
            enabled = false;
	}

    public void RegisterTower(GameObject go)
    {
        Towers.Add(go);
    }

    private void OnEnable()
    {
        EnableTowerDeleteComponents(true);
    }

    private void OnDisable()
    {
        Towers.RemoveAll(t => t == null);
        EnableTowerDeleteComponents(false);
    }

    void EnableTowerDeleteComponents(bool enable)
    {
        foreach (GameObject tower in Towers)
        {
            TowerDeleteComponent comp = tower.GetComponent<TowerDeleteComponent>();
            if (comp)
                comp.InDeleteMode = enable;
        }
    }
}

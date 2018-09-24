using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class PlaceTowerScript : MonoBehaviour {
    public Material GhostMaterial = null;
    public GameObject TowerZones = null;
    public GameObject GoldWarningPanel = null;
    public GameObject RangeIndicatorPrefab = null;
    public AudioClip PlaceTowerSound = null;

    //private:
    private GameObject PrefabToPlace = null; //this variable should come from a prefab
    private AudioSource Audio = null;
    private GameObject RangeIndicator = null;
    private GameObject GhostObject = null;
    private Color AllowedColor = new Color(0f, 1f, 0f, 0.3f);
    private Color NotAllowedColor = new Color(1f, 0f, 0f, 0.3f);
   
    private bool IsAllowedToPlace = false;
    private bool IsPaused = false;
    private bool ChangeGhost = true;
    // Use this for initialization
    void Start ()
    {
        Assert.IsNotNull(GhostMaterial, "PlaceTowerScript.Start: No Material for ghost form is provided");
        Assert.IsNotNull(TowerZones, "PlaceTowerScript.Start: No tower zones are defined");
        Assert.IsNotNull(GoldWarningPanel, "PlaceTowerScript.Start: No warning panel for gold is selected");
        Assert.IsNotNull(RangeIndicatorPrefab, "PlaceTowerScript.Start: No range idicator is selected");
        Assert.IsNotNull(PlaceTowerSound, "PlaceTowerScript.Start: No Place sound is set");

        Audio = GetComponent<AudioSource>();
        Assert.IsNotNull(Audio, "PlaceTowerScript.Start: No AudioSource component found!");

        //create range indicator and hide it
        RangeIndicator = Instantiate(RangeIndicatorPrefab);
        RangeIndicator.SetActive(false);

        enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsPaused)
            return;

        UpdatePlacingMode();
        CheckForPlacingEvent();
        MoveObjectToMouseCursor();
        ChangeObjectToPlaceColor((IsAllowedToPlace) ? AllowedColor : NotAllowedColor);
    }

    public void Pause(bool pause)
    {
        IsPaused = pause;
        IsAllowedToPlace = false;
    }
    public void StartPlacingTower(GameObject towerTemplate)
    {
        ChangeGhost = PrefabToPlace != towerTemplate;
        PrefabToPlace = towerTemplate;
    }

    private void UpdatePlacingMode()
    {
        if (Input.GetMouseButtonUp(1))
        {
            enabled = false;
        }

        if (ChangeGhost)
        {
            DestroyGhost();
            CreateNewGhost();
            ChangeGhost = false;
        }
    }

    private void OnDisable()
    {
        DestroyGhost();
        PrefabToPlace = null;
        UpdateTowerZoneStatus(false);
    }

    private void OnEnable()
    {
        UpdateTowerZoneStatus(true);
    }

    private void MoveObjectToMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        { 
            IsAllowedToPlace = (hitInfo.collider.tag == Utils.TowerZoneTag);
            Vector3 currPos = hitInfo.point;
            currPos.Set(currPos.x, hitInfo.transform.position.y, currPos.z);
            GhostObject.transform.position = currPos; //snap to y
        }
    }

    private void CheckForPlacingEvent()
    {
        if (Input.GetMouseButtonUp(0) && IsAllowedToPlace)
        {
            //first check if we have enough gold
            TowerComponent towerComp = PrefabToPlace.GetComponent<TowerComponent>();
            Assert.IsNotNull(towerComp, "PlaceTowerScript.StartPlacingTower Template is not a valid tower prefab");

            if (towerComp == null)
                return;

            int goldAfterPurchase = GameState.Gold - towerComp.Cost;
            if (goldAfterPurchase < 0) //not enough gold
            {
                ShowWarning();
                return;
            }
            GameState.Gold = goldAfterPurchase;

            GameObject newGO = Instantiate(PrefabToPlace, GhostObject.transform.position, GhostObject.transform.rotation);

            DeleteTowerScript deleteScript = GetComponent<DeleteTowerScript>();
            Assert.IsNotNull(deleteScript, "PowerTowerScript.CheckForPlacingEvent: Tower manager should have a DeleteTowerScript attached");
            deleteScript.RegisterTower(newGO);

            Audio.clip = PlaceTowerSound;
            Audio.Play();
        }
    }


    private void CreateNewGhost()
    {
        GhostObject = Instantiate(PrefabToPlace);
        
        Assert.IsNotNull(GhostObject, "PlaceTowerScript.CreateNewGhost: Failed to instantiate ghost object!");
        //disable the collider if any

        Collider collider = GhostObject.GetComponent<Collider>();
        if (collider)
        {
            collider.enabled = false;
        }

        TowerComponent towerComp = GhostObject.GetComponent<TowerComponent>();
        if (towerComp)
        {
            towerComp.enabled = false;
        }

        //if a particle system is in place hide it
        ParticleSystem partSystem = GhostObject.GetComponentInChildren<ParticleSystem>();
        if (partSystem)
        {
            partSystem.gameObject.SetActive(false);
        }

        //change prefab material to ghost material
        if (GhostMaterial != null)
        {
            Renderer[] renderComponents = GhostObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderComponents)
            {
                renderer.material = GhostMaterial;
                renderer.receiveShadows = false;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.lightProbeUsage = LightProbeUsage.Off;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }
        }

        //update range indicator
        RangeIndicator.SetActive(true);
        RangeIndicator.transform.localScale = new Vector3(towerComp.RangeOfFire, 1f, towerComp.RangeOfFire);
        RangeIndicator.transform.position = new Vector3(0f, 2f, 0f); //added an offset not to interact with the terrain
        RangeIndicator.transform.parent = GhostObject.transform;
    }

    private void DestroyGhost()
    {
        Destroy(GhostObject);
        RangeIndicator.SetActive(false);
        RangeIndicator.transform.parent = null;
    }

    private void ChangeObjectToPlaceColor(Color color)
    {
        if (GhostMaterial == null)
            return;

        Renderer[] renderComponents = GhostObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer comp in renderComponents)
            comp.material.SetColor("_Color", color);
    }

    private void UpdateTowerZoneStatus(bool enabled)
    {
        TowerZoneComponent[] towerZones = TowerZones.GetComponentsInChildren<TowerZoneComponent>();

        foreach (TowerZoneComponent zone in towerZones)
            zone.Activate(enabled);
    }

    private void ShowWarning()
    {
        GoldWarningPanel.SetActive(false);
        GoldWarningPanel.SetActive(true);
    }

}

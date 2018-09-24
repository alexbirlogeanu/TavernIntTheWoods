using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TooltipComponent : MonoBehaviour {
    private RectTransform TransformComp;
    private UnityEngine.UI.Text DamageValueText = null;
    private UnityEngine.UI.Text DamageTypeText = null;
    private UnityEngine.UI.Text SlowText = null;
    private UnityEngine.UI.Text RateOfFireText = null;
    private UnityEngine.UI.Text DOTText = null;
    private UnityEngine.UI.Text BlastText = null;
    private UnityEngine.UI.Text CostText = null;
    // Use this for initialization
    void Start () {
        gameObject.SetActive(false);
        TransformComp = transform as RectTransform;
        Assert.IsNotNull(TransformComp, "TooltipComponent.Start: Cast to Rect Transform failed");
        InitTextValues("DmgTextValue", out DamageValueText);
        InitTextValues("DmgTypeTextValue", out DamageTypeText);
        InitTextValues("SlowTextValue", out SlowText);
        InitTextValues("FireRateTextValue", out RateOfFireText);
        InitTextValues("DotTextValue", out DOTText);
        InitTextValues("BlastTextValue", out BlastText);
        InitTextValues("CostTextValue", out CostText);
    }


    // Update is called once per frame
    void Update () {
		
	}
    
    private void InitTextValues(string name, out UnityEngine.UI.Text text)
    {
        Transform t = TransformComp.Find(name);
        Assert.IsNotNull(t, "TooltipComponent.InitTextValue: Name not found: " + name);
        text = t.GetComponent<UnityEngine.UI.Text>();
        Assert.IsNotNull(t, "TooltipComponent.InitTextValue: No text component for : " + name);
    }

    public void Show(GameObject tower)
    {
        TowerComponent towerComp = tower.GetComponent<TowerComponent>();
        Assert.IsNotNull(towerComp, "TooltipComponent.Show: Argument is not a valid game object!");
        if (towerComp == null)
            return;


        FillTowerDetails(towerComp);
        BulletComponent projectile = towerComp.BulletPrefab.GetComponent<BulletComponent>();
        Assert.IsNotNull(projectile, "TooltipComponent.Show: Tower doesnt have a valid bullet prefab!");

        FillProjectileDetails(projectile);

        gameObject.SetActive(true);

        Vector3 OffsetVector = new Vector3(0f, 5f, 0.0f);
        TransformComp.position = Input.mousePosition + OffsetVector;
    }

    private void FillProjectileDetails(BulletComponent projectile)
    {
        DamageValueText.text = ((int)projectile.Damage).ToString();
        BlastText.text = projectile.BlastRadius.ToString();

        DamageTypeText.text = Utils.GetUIStringFromElementType(projectile.DamageType);
        DamageTypeText.color = Utils.GetUIColorFromElementType(projectile.DamageType);

        float slow = 0f;
        MoveModifierComponent moveMod = projectile.gameObject.GetComponent<MoveModifierComponent>();
        if (moveMod != null)
        {
            slow = moveMod.Modifier.Slow;
        }
        SlowText.text = slow.ToString();

        float dot = 0f;
        DamageModifierComponent dmgMod = projectile.gameObject.GetComponent<DamageModifierComponent>();
        if (dmgMod != null)
        {
            dot = dmgMod.Modifier.DamageOverTime;
        }
        DOTText.text = dot.ToString();
    }

    private void FillTowerDetails(TowerComponent tower)
    {
        RateOfFireText.text = tower.RateOfFire.ToString();

        Color costColor = Color.white;
        if (tower.Cost > GameState.Gold)
            costColor = Color.red;

        CostText.text = tower.Cost.ToString();
        CostText.color = costColor;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class EnemyStatus
{
    public float Health = 100;
    public int Bounty = 5;
    public EElementType Resistance = EElementType.None;
    public EElementType Weakness = EElementType.None;
}

public class EnemyStatusComponent : MonoBehaviour {

    public EnemyStatus Status = new EnemyStatus();
    public AnimationClip DeathAnimation = null;
    
    //private:
    private Dictionary<int, DamageModifier> Modifiers = new Dictionary<int, DamageModifier>();
    private Dictionary<int, GameObject> ModifierVisuals = new Dictionary<int, GameObject>(); //use to keep status visuals

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {

        ApplyModifiers();
        UpdateModifiers();

        if (IsDead())
            StartCoroutine(Kill());
    }

    public void OnDamage(float dmgValue, EElementType dmgType)
    {
        float multiplier = CalculateDamageMultiplier(dmgType);
        Status.Health -= dmgValue * multiplier;
    }

    public void ApplyModifier(DamageModifier dmgModifier)
    {
        //add new Modifier
        Assert.IsNotNull(dmgModifier, "EnemyMoveScript.ApplyModifier: Modifier should not be null");
        Modifiers[dmgModifier.Id] = dmgModifier;
        CreateStatusEffect(dmgModifier);

    }

    private void ApplyModifiers()
    {
        foreach (DamageModifier mod in Modifiers.Values)
        {
            float multiplier = CalculateDamageMultiplier(mod.DamageType);
            mod.StatusDuration -= Time.deltaTime;
            float damageValue = Time.deltaTime * mod.DamageOverTime * multiplier;
            Status.Health -= damageValue;
        }
    }

    private void UpdateModifiers()
    {
        //check for expired modifiers
        List<int> modifierToRemove = new List<int>();
        foreach(KeyValuePair<int, DamageModifier> entry in Modifiers)
        {
            if (entry.Value.StatusDuration <= 0f)
                modifierToRemove.Add(entry.Key);
        }

        //remove expired modifiers
        foreach (int key in modifierToRemove)
        {
            Modifiers.Remove(key);
            GameObject visual = null;
            if (ModifierVisuals.TryGetValue(key, out visual))
            {
                Assert.IsNotNull(visual, "EnemyStatusComponent.UpdateModifier: Visual is null. This should never happen. Key = " + key);
                visual.transform.parent = null;
                Destroy(visual);
                ModifierVisuals.Remove(key);
            }
        }
    }

    private float CalculateDamageMultiplier(EElementType dmgType)
    {
        float multiplier = 1.0f;

        if (dmgType != EElementType.None)
        {
            if (dmgType == Status.Resistance)
                multiplier = Utils.ResistanceMultiplier;
            if (dmgType == Status.Weakness)
                multiplier = Utils.WeaknessMultiplier;
        }

        return multiplier;
    }

    private void CreateStatusEffect(DamageModifier mod)
    {

        if (mod.ModifierEffect != null && !ModifierVisuals.ContainsKey(mod.Id))
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider)
            {
                Bounds bounds = collider.bounds;
                Vector3 startPos = bounds.center;
                //we have to offset y based on effect anchor
                startPos.y += mod.EffectAnchor * bounds.size.y / 2f;

                GameObject effect = Instantiate(mod.ModifierEffect, startPos, Quaternion.identity);
                effect.transform.parent = gameObject.transform; //set the parrent to the effect
                Assert.IsNotNull("EnemyStatusComponent.CreateStatusEffect: Couldn't instantiate status effect");

                ModifierVisuals[mod.Id] = effect;
            }
        }
    }

    IEnumerator Kill()
    {
        //increment global gold
        enabled = false;
        GameState.Gold += Status.Bounty;
        if (DeathAnimation)
        {
            Animation anim = gameObject.GetComponentInChildren<Animation>();
            if (anim)
            {
                anim.Play(DeathAnimation.name);
                yield return new WaitForSeconds(DeathAnimation.length + 0.5f);
            }

        }

        Animator animController = gameObject.GetComponentInChildren<Animator>();
        if (animController != null && animController.runtimeAnimatorController != null)
        {
            animController.SetBool("Die", true);
            yield return new WaitForSeconds(3.0f); //nice coding. but whatever
        }

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return Status.Health <= 0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageModifier
{
    public static int None = -1;

    [Tooltip("Id of the DamageModifier. Is used to refresh the effect if an enemy is already affected")]
    public int Id = -1;//this is kinda ugly, to let the designer to set manualy. it could be done with a table of statuses to assign unique ids, but no time now

    [Tooltip("apply a status or movement effect if > 0")]
    public float DamageOverTime = 0f;

    [Tooltip("apply a status effect if statusDuration > 0")]
    public float StatusDuration = 0f;

    public GameObject ModifierEffect = null;

    [Tooltip("it is used to anchor the effect relative to object collider. 0 means at the position of the collider, usually at the middle of the character")]
    [Range(-2f, 2f)]
    public float EffectAnchor = 0f; //it is used to anchor the effect relative to object collider. 0 means at the position of the collider, usually at the middle of the character. Number is a percent and compute the distance based on the size of the colider box

    [HideInInspector]
    public EElementType DamageType  = EElementType.None;

    private DamageModifier()
    {
        Id = -1;
        DamageOverTime = 0f;
        StatusDuration = 0f;
        ModifierEffect = null;
        EffectAnchor = 0f; 
        DamageType = EElementType.None;
    }

    public bool CanBeApplied()
    {
        return StatusDuration > 0f && DamageOverTime > 0f;
    }
};

public class DamageModifierComponent : MonoBehaviour
{
    public DamageModifier Modifier;

    public void ApplyOnTarget(EnemyStatusComponent target, EElementType type)
    {
        if (Modifier.Id != DamageModifier.None && Modifier.CanBeApplied())
        {
            Modifier.DamageType = type;
            target.ApplyModifier(Modifier);
        }
    }
}

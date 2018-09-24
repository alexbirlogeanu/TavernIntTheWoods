using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveModifier
{
    public static int None = -1;

    [Tooltip("Id of the DamageModifier. Is used to refresh the effect if an enemy is already affected")]
    public int Id = -1; //this is kinda ugly, to let the designer to set manualy. it could be done with a table of statuses to assign unique ids, but no time now

    [Tooltip("apply a  movement effect if > 0")]
    public float StatusDuration = 0f;

    [Tooltip("apply a status effect if statusDuration > 0")]
    [Range(0f, 1f)]
    public float Slow = 0f;

    private MoveModifier()
    {
        Id = None;
        StatusDuration = 0;
        Slow = 0;
    }

    public bool CanBeApplied()
    {
        return StatusDuration > 0f && Slow > 0f;
    }
};

public class MoveModifierComponent : MonoBehaviour {
    public MoveModifier Modifier;
    // Use this for initialization
    void Start ()
    {
		
	}
	
    public void ApplyOnTarget(EnemyMoveScript target)
    {
        if (Modifier.Id != MoveModifier.None && Modifier.CanBeApplied())
        {
            target.ApplyModifier(Modifier);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Utils
{
    static public int EnemyLayerMask = 1 << 8;
    //tags
    static public string EnemyTag = "Enemy";
    static public string TowerZoneTag = "TowerZone";
    static public string FinishTag = "Finish";
    static public string CheckpointTag = "Checkpoint";
    static public float ResistanceMultiplier = 0.75f;
    static public float WeaknessMultiplier = 1.5f;
    static public Color GetUIColorFromElementType(EElementType type)
    {
        switch(type)
        {
            case EElementType.Rock:
                return new Color(0.35f, 0.23f, 0.07f);
            case EElementType.Fire:
                return Color.red;
            case EElementType.Venom:
                return Color.green;
            case EElementType.Ice:
                return Color.blue;
            default:
                return Color.black;
        }
    }

    static public string GetUIStringFromElementType(EElementType type)
    {
        switch (type)
        {
            case EElementType.Rock:
                return ("Earth").ToString();
            case EElementType.Fire:
                return ("Fire").ToString();
            case EElementType.Venom:
                return ("Poison").ToString();
            case EElementType.Ice:
                return ("Ice").ToString();
            default:
                return ("None").ToString();
        }
    }
}

public enum EElementType
{
    None,
    Rock,
    Venom,
    Fire,
    Ice
}
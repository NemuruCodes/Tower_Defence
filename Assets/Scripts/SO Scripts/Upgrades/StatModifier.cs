using System;
using System.Collections.Generic;
using UnityEngine;

public enum TowerStat { Damage, Range, MaxHealth, FireRate,
    SellAmount
}
public enum ModifierMode { Flat, PercentAdd }

[Serializable]
public struct StatModifier
{
    public TowerStat stat;
    public ModifierMode mode;
    public float value;   // Flat: +5 | PercentAdd: 0.2 = +20%
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    private TowerData data;
    private readonly List<StatModifier> modifiers = new();

    public event Action<TowerStat, float, float> OnStatChanged; //stat , old Value, New Value

    public void Init(TowerData towerData) => data = towerData;

    public float Damage => Get(TowerStat.Damage, data.damage);
    public float Range => Get(TowerStat.Range, data.range);
    public float MaxHealth => Get(TowerStat.MaxHealth, data.maxHealth);
    public float FireRate => Get(TowerStat.FireRate, data.fireRate);
    public int SellAmount=> (int)Get(TowerStat.SellAmount, data.sellAmount);

    public void AddModifiers(IEnumerable<StatModifier> newMods)
    {
        // Snapshot old values so listeners can react to the delta
        var stats = (TowerStat[])Enum.GetValues(typeof(TowerStat));
        var old = new Dictionary<TowerStat, float>();
        foreach (var s in stats) old[s] = GetByEnum(s);

        modifiers.AddRange(newMods);

        foreach (var s in stats)
        {
            float now = GetByEnum(s);
            if (!Mathf.Approximately(old[s], now))
                OnStatChanged?.Invoke(s, old[s], now);
        }
    }
    private float GetByEnum(TowerStat s) => s switch
    {
        TowerStat.Damage => Damage,
        TowerStat.Range => Range,
        TowerStat.MaxHealth => MaxHealth,
        TowerStat.FireRate => FireRate,
        TowerStat.SellAmount => SellAmount,
        _ => 0f
    };

    private float Get(TowerStat stat, float baseValue)
    {
        float flat = 0f, percent = 0f;
        foreach (var m in modifiers)
        {
            if (m.stat != stat) continue;
            if (m.mode == ModifierMode.Flat) flat += m.value;
            else percent += m.value;
        }
        return (baseValue + flat) * (1f + percent);
    }
}

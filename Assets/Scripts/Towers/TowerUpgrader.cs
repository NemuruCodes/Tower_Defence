using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrader : MonoBehaviour
{
    [SerializeField] private TowerStats stats;
    [SerializeField] private TowerVisuals visuals;

    private TowerData data;
    private readonly Dictionary<UpgradePathSO, int> tiers = new();

    public int TotalSpent { get; private set; }

    public event Action<UpgradePathSO, int> OnUpgraded; //per-tower event (UI listens to this)

    public static event Action<TowerUpgrader, UpgradePathSO, UpgradeSO> AnyUpgraded; // Global event (Upgrade Tracker listens to this)

    public float UpgradePower { get; private set; }
    public IReadOnlyList<UpgradePathSO> Paths => data.upgradePaths;

    public void Init(TowerData towerData)
    {
        data = towerData;
    }
    public int GetTier(UpgradePathSO path) => tiers.TryGetValue(path, out int t) ? t : 0;

    //public void Upgrade(UpgradePathSO path) => TryUpgrade(path);

    public UpgradeSO GetNext(UpgradePathSO path)
    {
        int t = GetTier(path);
        return t < path.tiers.Count ? path.tiers[t] : null;   // null = maxed
    }

    public bool TryUpgrade(UpgradePathSO path)
    {
        UpgradeSO next = GetNext(path);
        if (next == null) return false;

        // Swap for however your build-cost currency works
        if (!ResourceManager.Instance.Spend(next.cost)) return false;

        TotalSpent += next.cost;

        stats.AddModifiers(next.modifiers);
        tiers[path] = GetTier(path) + 1;
        UpgradePower += next.difficultyWeight;

        visuals?.ShowTier(path, tiers[path]);
        OnUpgraded?.Invoke(path, tiers[path]);
        AnyUpgraded?.Invoke(this, path, next);

        Debug.Log($"The Current Sell Amount for " + data.towerName + " is " + stats.SellAmount);
        
        return true;
    }

   


}

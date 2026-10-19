using System.Collections.Generic;
using UnityEngine;

public class UpgradeTracker : MonoBehaviour
{
    public static UpgradeTracker Instance { get; private set; }

    public int TotalUpgradesBought { get; private set; }
    private readonly Dictionary<UpgradePathSO, int> perPath = new();

    private void Awake() => Instance = this;
    private void OnEnable() => TowerUpgrader.AnyUpgraded += OnUpgraded;
    private void OnDisable() => TowerUpgrader.AnyUpgraded -= OnUpgraded;

    private float totalPower;

    private void OnUpgraded(TowerUpgrader tower, UpgradePathSO path, UpgradeSO upgrade)
    {
        TotalUpgradesBought++;
        totalPower += upgrade.difficultyWeight;
        perPath[path] = (perPath.TryGetValue(path, out int n) ? n : 0) + 1;
    }

    public float GetUpgradePower() => totalPower;
    public int GetUpgradesInPath(UpgradePathSO path) => perPath.TryGetValue(path, out int n) ? n : 0;

}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Upgrade Path")]
public class UpgradePathSO : ScriptableObject
{
    public string displayName;      // "Damage", "Range", "Health"
    public Sprite icon;
    public List<UpgradeSO> tiers = new();   // tier 1, 2, 3...
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public int cost;
    [Tooltip("How much this upgrade counts toward difficulty scaling")]
    public float difficultyWeight = 1f;
    public List<StatModifier> modifiers = new();
}

using UnityEngine;

//https://learn.unity.com/tutorial/strategy-pattern

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Identity")]
    public string towerName;
    public GameObject towerPrefab;

    [Header("Stats")]
    public float maxHealth = 100;
    public float damage = 5f;
    public float range = 10f;
    public float fireRate = 1f; //shots per-second
    public int buildCost = 50;

    [Header("Behaviour")]
    public ProjectileBehaviour projectileBehavior;
    public TargetingStratedgy targetingStrategy;

    
    
}

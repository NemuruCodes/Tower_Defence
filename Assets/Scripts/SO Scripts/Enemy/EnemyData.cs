using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public GameObject prefab;

    [Header("Health")]
    public float maxHealth = 50f;
    public int resourceReward = 50;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Combat")]
    public float towerDetectionRange = 6f;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackInterval = 1f;
}

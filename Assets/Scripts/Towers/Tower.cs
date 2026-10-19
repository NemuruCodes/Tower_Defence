using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://chandler-lane.medium.com/tower-defense-architecture-in-unity-dynamic-tower-targeting-cdcf79d404c9
//https://learn.unity.com/tutorial/strategy-pattern
//https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html

[RequireComponent(typeof(TowerStats))]      
[RequireComponent(typeof(TowerUpgrader))]   
public class Tower : MonoBehaviour, IDamageable
{

    [SerializeField] private TowerData data;
    [SerializeField] private RangeIndicator rangeIndicator;
    [SerializeField] private Transform firePoint;

    private float currentHealth;
    private float fireCooldown;
    private TargetingStratedgy targeting;
    private ProjectileBehaviour attackEffect;
    public bool showRangeGizmo = false;
    [SerializeField] private LayerMask enemyMask;

    private IDamageable currentTarget;

    private TowerStats stats;
    private TowerUpgrader upgrader;

    public float CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0f;

    public event Action OnDeath;

    [SerializeField] private HealthBar healthBar;

    public bool isCenterTower;

    [SerializeField] private DamageFlash damageFlash;

    private static readonly List<Tower> activeTowers = new List<Tower>();

    private float CurrentRange => stats != null ? stats.Range : data.range;
    /*
    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
        if (isCenterTower) 
        {
            healthBar.enabled = true;
        }
        else
        {
            healthBar.enabled = false;
        }
           
    }
    */
    private void Awake()
    {
        stats = GetComponent<TowerStats>();
        upgrader = GetComponent<TowerUpgrader>();
        stats.Init(data);
        upgrader.Init(data);
        stats.OnStatChanged += HandleStatChanged;

        currentHealth = stats.MaxHealth;
        targeting = data.targetingStrategy;   // SO implements the interface
        attackEffect = data.projectileBehavior;

        rangeIndicator.SetRadius(stats.Range);
        healthBar.SetHealth(currentHealth, stats.MaxHealth);

        
    }

    private void HandleStatChanged(TowerStat stat, float oldValue, float newValue)
    {
        switch (stat)
        {
            case TowerStat.Range:
                rangeIndicator.SetRadius(newValue);
                break;

            case TowerStat.MaxHealth:
                currentHealth += newValue - oldValue;    // reward the upgrade
                healthBar.SetHealth(currentHealth, newValue);
                break;

                // Damage and FireRate need no handling, they're read at use time
        }
    }

    public static int PlacedTowerCount
    {
        get
        {
            int count = 0;
            foreach (Tower t in activeTowers)
                if (!t.CompareTag("MainTower"))
                    count++;
            return count;
        }
    }

    private void OnEnable()
    {
        activeTowers.Add(this);
    }

    private void OnDisable()
    {
        activeTowers.Remove(this);
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown > 0f) return;

        var enemiesInRange = Physics.OverlapSphere(transform.position, data.range, enemyMask)
        .Select(c => c.transform);

        var target = targeting.SelectTarget(transform.position, data.range, enemiesInRange);
        if (target == null) return;

        Attack(target);
        fireCooldown = 1f / stats.FireRate;
    }



    private void Attack(Transform target)
    {
        attackEffect.Fire(firePoint, target, stats.Damage);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth, stats.MaxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (damageFlash != null) damageFlash.Flash();
    }

   

    private void Die()
    {
        OnDeath?.Invoke();

       

        Destroy(gameObject);
    }

    public void Select() => rangeIndicator.Show();
    public void Deselect() => rangeIndicator.Hide();


    private void OnDrawGizmos()
    {
        if (!showRangeGizmo || data == null) return;
        DrawRangeGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        DrawRangeGizmo();
    }

    private void DrawRangeGizmo()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, data.range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}

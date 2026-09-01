using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [SerializeField] private TowerData data;
    [SerializeField] private RangeIndicator rangeIndicator;

    private float currentHealth;
    private float fireCooldown;
    private TargetingStratedgy targeting;
    private ProjectileBehaviour attackEffect;
    public bool showRangeGizmo = false;

    private void Awake()
    {
        currentHealth = data.maxHealth;
        targeting = data.targetingStrategy;   // SO implements the interface
        attackEffect = data.projectileBehavior;

        rangeIndicator.SetRadius(data.range);
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown > 0f) return;

        var enemiesInRange = Physics.OverlapSphere(transform.position, data.range)
            .Select(c => c.transform);

        var target = targeting.SelectTarget(transform.position, data.range, enemiesInRange);
        if (target == null) return;

        Attack(target);
        fireCooldown = 1f / data.fireRate;
    }

    private void Attack(Transform target)
    {
        // fire projectile, then on impact call:
        attackEffect.OnHit(target, data.damage, target.position);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Destroy(gameObject);
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

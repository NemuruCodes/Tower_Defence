using UnityEngine;

[CreateAssetMenu(fileName = "PhysicalProjectileBehaviour", menuName = "Scriptable Objects/PhysicalProjectileBehaviour")]
public class PhysicalProjectileBehaviour : ProjectileBehaviour
{
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private HitVisual hitVisual;
    [SerializeField] private float duration = 0.5f;

    [SerializeField] private float speed = 15f;

    public override void OnHit(Transform target, float damage, Vector3 hitPosition)
    {
        if (target != null && target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            
        }

        if (hitVisual != null)
        {

            HitVisual visual = Instantiate(hitVisual);
            visual.Play(hitPosition, duration);
        }
            
    }

    public override void Fire(Transform firePoint, Transform target, float damage)
    {
        Projectile p = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        p.Launch(target, speed, damage, this);

    }

    // Called by Projectile once it reaches the target
    public void ResolveHit(Transform target, float damage, Vector3 hitPosition) =>
        OnHit(target, damage, hitPosition);
}

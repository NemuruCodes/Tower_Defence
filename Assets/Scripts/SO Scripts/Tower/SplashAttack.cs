using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/Effects/SplashAttack")]
public class SplashAttack : ProjectileBehaviour
{
    public float radius = 2f;
    public LayerMask enemyMask;

    public override void OnHit(Transform target, float damage, Vector3 hitPosition)
    {
        foreach (var h in Physics.OverlapSphere(hitPosition, radius, enemyMask))
            h.GetComponent<EnemyHealth>()?.TakeDamage(damage);
    }
}

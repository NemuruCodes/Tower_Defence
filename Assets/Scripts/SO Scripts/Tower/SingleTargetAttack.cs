using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetAttack", menuName = "TowerDefence/Effects/SingleTargetAttack")]
public class SingleTargetAttack : ProjectileBehaviour
{
    public override void OnHit(Transform target, float damage, Vector3 hitPosition)
    {
        target.GetComponent<EnemyHealth>()?.TakeDamage(damage);
    }
}

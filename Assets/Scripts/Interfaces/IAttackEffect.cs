using UnityEngine;

public interface IAttackEffect
{
    void OnHit(Transform target, float damage, Vector3 hitPosition);
}

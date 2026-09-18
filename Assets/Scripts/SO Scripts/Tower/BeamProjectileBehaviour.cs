using UnityEngine;

[CreateAssetMenu(fileName = "BeamProjectileBehaviour", menuName = "Scriptable Objects/Beam")]
public class BeamProjectileBehaviour : ProjectileBehaviour
{
    [SerializeField] private BeamVisual beamPrefab;
    [SerializeField] private float beamVisualDuration = 0.1f;

    public override void OnHit(Transform target, float damage, Vector3 hitPosition)
    {
        if (target != null && target.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(damage);
    }

    public override void Fire(Transform firePoint, Transform target, float damage)
    {
        OnHit(target, damage, target.position);

        BeamVisual beam = Instantiate(beamPrefab);
        beam.Play(firePoint.position, target.position, beamVisualDuration);
    }
}

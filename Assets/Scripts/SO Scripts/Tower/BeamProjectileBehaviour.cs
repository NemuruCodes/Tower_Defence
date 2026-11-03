using UnityEngine;

[CreateAssetMenu(fileName = "BeamProjectileBehaviour", menuName = "Scriptable Objects/Beam")]
public class BeamProjectileBehaviour : ProjectileBehaviour
{
    [SerializeField] private BeamVisual beamPrefab;
    [SerializeField] private float beamLingerTime = 0.5f;

    public override void OnHit(Transform target, float damage, Vector3 hitPosition)
    {
        if (target != null && target.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(damage);
    }

    public override void Fire(Transform firePoint, Transform target, float damage)
    {
        if (target == null) return;

        OnHit(target, damage, target.position);

        // Reuse this tower's beam if it already has one
        BeamVisual beam = firePoint.GetComponentInChildren<BeamVisual>();
        if (beam == null)
            beam = Instantiate(beamPrefab, firePoint);

        beam.Track(firePoint, target, beamLingerTime);
    }
}

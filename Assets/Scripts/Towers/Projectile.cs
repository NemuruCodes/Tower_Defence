using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    private Transform target;
    private float speed;
    private float damage;

    private float lifetime = 10f;

    private float duration = 0.5f;

    private PhysicalProjectileBehaviour source;

    

    public void Launch(Transform target, float speed, float damage, PhysicalProjectileBehaviour source)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.source = source;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) { Destroy(gameObject); return; }

        if (target == null) { Destroy(gameObject); return; }

        Vector3 dir = target.position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        // layer filter so it only resolves against enemies, not other towers/ground/etc
        if ((enemyMask.value & (1 << other.gameObject.layer)) == 0) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            source.ResolveHit(other.transform, damage, transform.position);

            Destroy(gameObject);
        }
    }
}

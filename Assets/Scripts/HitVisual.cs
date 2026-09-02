using UnityEngine;

public class HitVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem impactVFX;

    public void Play(Vector3 end, float duration)
    {

        if (impactVFX != null)
        {
            impactVFX.transform.position = end;
            impactVFX.Play();
        }

        Destroy(gameObject, duration);
    }
}

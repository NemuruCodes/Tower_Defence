using UnityEngine;

public class BeamVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem impactVFX; // optional, can be null

    public void Play(Vector3 start, Vector3 end, float duration)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        if (impactVFX != null)
        {
            impactVFX.transform.position = end;
            impactVFX.Play();
        }

        Destroy(gameObject, duration);
    }
}

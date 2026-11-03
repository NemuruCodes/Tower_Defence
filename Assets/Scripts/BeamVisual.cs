using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class BeamVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem impactVFX; // optional, can be null
    [SerializeField] private VisualEffect effect;
    public bool UseVFX = false;

    private Transform start;
    private Transform target;
    private float expireTime;

    public void Track(Transform firePoint, Transform newTarget, float lingerTime)
    {
        start = firePoint;
        target = newTarget;
        expireTime = Time.time + lingerTime;

        if (!UseVFX)
        {
            if (impactVFX != null) impactVFX.Play();
        }
        else if (effect != null)
        {
            effect.Play();
        }
    }

    private void LateUpdate()
    {
        if (start == null || target == null || !target.gameObject.activeInHierarchy || Time.time >= expireTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 end = target.position;
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end);

        if (impactVFX != null) impactVFX.transform.position = end;
        if (effect != null) effect.transform.position = end;
    }
    /*
    public void Play(Vector3 start, Vector3 end, float duration)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        if (UseVFX == false)
        {
            if (impactVFX != null)
            {
                impactVFX.transform.position = end;
                impactVFX.Play();
            }

        }
        else if (effect != null)
        {
            effect.transform.position = end;
            effect.Play();
        }

        Destroy(gameObject, duration);
    }
    */
}

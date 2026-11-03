using UnityEngine;
using UnityEngine.VFX;

public class HitVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem impactVFX;
    [SerializeField] private VisualEffect effect;
    public bool UseVFX = false;

    public void Play(Vector3 end, float duration)
    {
        if(UseVFX == false)
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
}

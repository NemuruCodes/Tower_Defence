using UnityEngine;
using System.Collections;
public class ShaderExplosion : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float maxSize = 16f;
    [SerializeField] private float minSize = 0.5f;

    private void Awake()
    {
        StartCoroutine(ScaleSphere());
        //StartCoroutine(UnScaleSphere());

    }

    private IEnumerator ScaleSphere()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = timer / duration;

            transform.localScale = Vector3.one * Mathf.Lerp(0f, maxSize, progress);

            yield return null;
        }

        transform.localScale = Vector3.one * maxSize;

        //Destroy(gameObject);

        // UnScaleSphere();
        StartCoroutine(UnScaleSphere());
    }

    private IEnumerator UnScaleSphere()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = timer / duration;

            transform.localScale = Vector3.one * Mathf.Lerp(maxSize, minSize, progress);

            yield return null;
        }

        transform.localScale = Vector3.one * minSize;

        Destroy(gameObject);
    }
}

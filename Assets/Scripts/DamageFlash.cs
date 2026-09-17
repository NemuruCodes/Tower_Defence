using UnityEngine;
using System;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Renderer[] renderersToFlash;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.15f;

    private MaterialPropertyBlock propBlock;
    private Coroutine flashRoutine;
    private Color[] baseColors;
    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor"); // URP

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();

        if (renderersToFlash == null || renderersToFlash.Length == 0)
            renderersToFlash = GetComponentsInChildren<Renderer>();

        baseColors = new Color[renderersToFlash.Length];
        for (int i = 0; i < renderersToFlash.Length; i++)
            baseColors[i] = renderersToFlash[i].sharedMaterial.color;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            for (int i = 0; i < renderersToFlash.Length; i++)
            {
                Color lerped = Color.Lerp(flashColor, baseColors[i], t / flashDuration);
                SetColor(renderersToFlash[i], lerped);
            }
            yield return null;
        }

        for (int i = 0; i < renderersToFlash.Length; i++)
            SetColor(renderersToFlash[i], baseColors[i]);

        flashRoutine = null;
    }

    private void SetColor(Renderer rend, Color color)
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorProperty, color);
        rend.SetPropertyBlock(propBlock);
    }
}

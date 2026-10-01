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
    private Color[][] baseColors;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();

        if (renderersToFlash == null || renderersToFlash.Length == 0)
            renderersToFlash = GetComponentsInChildren<Renderer>();

        baseColors = new Color[renderersToFlash.Length][];
        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            Material[] mats = renderersToFlash[i].sharedMaterials;
            baseColors[i] = new Color[mats.Length];

            for (int m = 0; m < mats.Length; m++)
            {
                if (mats[m] == null) { baseColors[i][m] = Color.white; continue; }

                if (mats[m].HasProperty(BaseColorId))
                    baseColors[i][m] = mats[m].GetColor(BaseColorId);
                
                else
                    baseColors[i][m] = Color.white;
            }
        }


        // if (renderersToFlash == null || renderersToFlash.Length == 0)
        //    renderersToFlash = GetComponentsInChildren<Renderer>();

        //baseColors = new Color[renderersToFlash.Length];
        //for (int i = 0; i < renderersToFlash.Length; i++)
        //baseColors[i] = renderersToFlash[i].sharedMaterial.color;
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
            float k = Mathf.Clamp01(t / flashDuration);
            for (int i = 0; i < renderersToFlash.Length; i++)
            {
                for (int m = 0; m < baseColors[i].Length; m++)
                {
                    Color lerped = Color.Lerp(flashColor, baseColors[i][m], k);
                    SetColor(renderersToFlash[i], m, lerped);
                }
                //Color lerped = Color.Lerp(flashColor, baseColors[i], t / flashDuration);
                //SetColor(renderersToFlash[i], lerped);
            }
            yield return null;
        }

        //for (int i = 0; i < renderersToFlash.Length; i++)
        // SetColor(renderersToFlash[i], baseColors[i]);

        ClearAll();
        flashRoutine = null;
    }

    /*private void SetColor(Renderer rend, Color color)
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorProperty, color);
        rend.SetPropertyBlock(propBlock);
    }*/

    private void SetColor(Renderer rend, int materialIndex, Color color)
    {
        rend.GetPropertyBlock(propBlock, materialIndex);
        propBlock.SetColor(BaseColorId, color);
        //propBlock.SetColor(LegacyColorId, color);
        rend.SetPropertyBlock(propBlock, materialIndex);
    }

    private void ClearAll()
    {
        for (int i = 0; i < renderersToFlash.Length; i++)
        {
            for (int m = 0; m < baseColors[i].Length; m++)
            {
                propBlock.Clear();
                renderersToFlash[i].SetPropertyBlock(propBlock, m);
            }
        }
    }
}



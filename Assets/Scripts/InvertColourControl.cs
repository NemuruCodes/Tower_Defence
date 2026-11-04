using UnityEngine;
using System.Collections;

public class InvertColourControl : MonoBehaviour
{
    [SerializeField] private Material invertMaterial; 
    [SerializeField] private float duration = 0.1f;

    private static readonly int InvertAmountID = Shader.PropertyToID("_InvertNum");
    private Coroutine running;

    public void Flash()
    {
        if (running != null)
            StopCoroutine(running); // Safe gaurd for multi hits and will restart cleanly if triggered again mid-flash

        running = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        invertMaterial.SetFloat(InvertAmountID, 1f);
        yield return new WaitForSeconds(duration);
        invertMaterial.SetFloat(InvertAmountID, 0f);
        running = null;
    }

    private void OnDisable()
    {
        invertMaterial.SetFloat(InvertAmountID, 0f); // don't leave the asset inverted
    }
}

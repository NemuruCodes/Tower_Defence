using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private bool billboardToCamera = true;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
            canvas.worldCamera = cam;
    }

    private void LateUpdate()
    {
        if (billboardToCamera && cam != null)
            transform.rotation = cam.transform.rotation;
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = max > 0f ? current / max : 0f;
    }
}

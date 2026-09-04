using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private bool billboardToCamera = true;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (billboardToCamera && cam != null)
            transform.forward = cam.transform.forward;
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = max > 0f ? current / max : 0f;
    }
}

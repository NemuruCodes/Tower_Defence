using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class AlwaysOnTop : MonoBehaviour
{
    [SerializeField] private bool billboardToCamera = false;

    private Camera cam;
    private Material overlayMaterial;

    private void Awake()
    {
        cam = Camera.main;

        overlayMaterial = new Material(Graphic.defaultGraphicMaterial);
        overlayMaterial.SetInt("unity_GUIZTestMode", (int)CompareFunction.Always);

        foreach (var graphic in GetComponentsInChildren<Graphic>(true))
        {
            if (graphic is TMP_Text tmp)
            {
                Material textMat = tmp.fontMaterial; // per-object instance
                textMat.SetInt("unity_GUIZTestMode", (int)CompareFunction.Always);
                tmp.fontMaterial = textMat;
            }
            else
            {
                graphic.material = overlayMaterial;
            }
        }
    }

    private void LateUpdate()
    {
        if (billboardToCamera && cam != null)
            transform.rotation = cam.transform.rotation;
    }

    private void OnDestroy()
    {
        if (overlayMaterial != null)
            Destroy(overlayMaterial);
    }
}

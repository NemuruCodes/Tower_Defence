using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] private int segments = 64;
    [SerializeField] private float yOffset = 0.05f;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.positionCount = segments;
        line.widthMultiplier = 0.1f;
        Hide();
    }

    public void SetRadius(float radius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, yOffset, Mathf.Sin(angle) * radius));
        }
    }

    public void Show() => line.enabled = true;
    public void Hide() => line.enabled = false;
}

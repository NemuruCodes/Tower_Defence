using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;

    [Header("Spinning")]
    public float spinSpeed = 90f; // Degrees per second

    private Vector3 startLocalPosition;

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Bob relative to the parent so the orb travels with the tower
        float newY = startLocalPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);

        // Spin around the orb's own Y axis
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }
}

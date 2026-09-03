using UnityEngine;

[DisallowMultipleComponent]
public class OrbitCameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The point the camera orbits around")]
    [SerializeField] private Transform target;

    [Tooltip("Find the object with this tag when the scene starts")]
    [SerializeField] private string targetTag = "MainTower";

    [Header("Orbit Settings")]
    [Tooltip("Degrees per second when holding A/D.")]
    [SerializeField] private float rotationSpeed = 60f;

    [SerializeField] private bool smoothRotation = true;

    [SerializeField] private float rotationSmoothTime = 0.15f;

    [Tooltip("Invert the A/D direction.")]
    [SerializeField] private bool invertRotationDirection = false;

    [Header("Distance / Zoom")]
    [SerializeField] private float distance = 15f;

    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 30f;

    [Tooltip("Allow zooming with the mouse scroll wheel.")]
    [SerializeField] private bool allowZoom = true;

    [SerializeField] private float zoomSpeed = 10f;

    [Header("Height / Pitch")]
    [Range(5f, 85f)]
    [SerializeField] private float pitchAngle = 45f;

    [Tooltip("Allow adjusting pitch with W/S or another input")]
    [SerializeField] private bool allowPitchAdjust = false;

    [SerializeField] private float pitchSpeed = 40f;
    [SerializeField] private float minPitch = 15f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Input")]
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode pitchUpKey = KeyCode.Q;
    [SerializeField] private KeyCode pitchDownKey = KeyCode.E;

    // Internal state
    private float currentYaw;
    private float currentRotationVelocity;
    private float rotationVelocitySmoothRef;

    private void Start()
    {
        if (target == null)
        {
            FindTargetByTag();
        }

        if (target == null)
        {
            Debug.LogWarning($"{nameof(OrbitCameraController)}: No target assigned and no object ");
            return;
        }

        // Initialize yaw from the camera's current position relative to the target, so it doesn't snap on first frame
        
        if (target != null)
        {
            Vector3 offset = transform.position - target.position;
            currentYaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        }
    }

    private void FindTargetByTag()
    {
        if (string.IsNullOrEmpty(targetTag)) return;

        GameObject found = GameObject.FindGameObjectWithTag(targetTag);
        if (found != null)
        {
            target = found.transform;
        }
    }

    private void Update()
    {
        if (target == null) return;

        HandleRotationInput();
        HandleZoomInput();
        HandlePitchInput();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        ApplyCameraTransform();
    }

    private void HandleRotationInput()
    {
        float direction = 0f;
        if (Input.GetKey(rotateLeftKey)) direction -= 1f;
        if (Input.GetKey(rotateRightKey)) direction += 1f;

        if (invertRotationDirection) direction *= -1f;

        float targetRotationSpeed = direction * rotationSpeed;

        if (smoothRotation)
        {
            currentRotationVelocity = Mathf.SmoothDamp(
                currentRotationVelocity,
                targetRotationSpeed,
                ref rotationVelocitySmoothRef,
                rotationSmoothTime);
        }
        else
        {
            currentRotationVelocity = targetRotationSpeed;
        }

        currentYaw += currentRotationVelocity * Time.deltaTime;
    }

    private void HandleZoomInput()
    {
        if (!allowZoom) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void HandlePitchInput()
    {
        if (!allowPitchAdjust) return;

        float pitchDirection = 0f;
        if (Input.GetKey(pitchUpKey)) pitchDirection -= 1f;
        if (Input.GetKey(pitchDownKey)) pitchDirection += 1f;

        pitchAngle += pitchDirection * pitchSpeed * Time.deltaTime;
        pitchAngle = Mathf.Clamp(pitchAngle, minPitch, maxPitch);
    }

    private void ApplyCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(pitchAngle, currentYaw, 0f);
        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + desiredOffset;

        transform.position = desiredPosition;
        transform.LookAt(target.position);
    }

    
    // Lets other scripts reassign the orbit target at runtime
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Vector3 offset = transform.position - target.position;
            currentYaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arrivalThreshold = 0.1f;

    [Header("Gravity / Jump")]
    public float gravity = -20f;
    public float jumpSpeed = 8f;
    public float groundedSnapThreshold = 0.05f;

    [Tooltip("Any upward step bigger than this triggers a jump instead of just walking up.")]
    public float jumpStepThreshold = 0.6f;

    [Tooltip("How close (horizontally) to the next cell before the jump is triggered.")]
    public float jumpTriggerDistance = 0.4f;

    /// <summary>Invoked once the enemy reaches the final waypoint (the Target).</summary>
    public System.Action OnTargetReached;

    private TerrainGrid grid;
    private List<Vector3> waypoints;
    private int waypointIndex;
    private float verticalVelocity;
    private bool grounded;
    private bool hasJumpedForCurrentSegment;

    public void Initialize(TerrainGrid terrainGrid, List<Vector2Int> path)
    {
        grid = terrainGrid;
        waypoints = BuildWaypoints(path);
        waypointIndex = 0;
        verticalVelocity = 0f;
        grounded = true;
        hasJumpedForCurrentSegment = false;

        if (waypoints.Count > 0)
            transform.position = waypoints[0];
    }

    private List<Vector3> BuildWaypoints(List<Vector2Int> path)
    {
        var result = new List<Vector3>(path.Count);
        foreach (Vector2Int cell in path)
        {
            float y = grid.GetSurfaceHeight(cell.x, cell.y) + 1f; // stand on top of the surface block
            result.Add(new Vector3(cell.x, y, cell.y));
        }
        return result;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0 || waypointIndex >= waypoints.Count)
            return;

        MoveTowardCurrentWaypoint();
        ApplyGravityAndGround();
    }

    private void MoveTowardCurrentWaypoint()
    {
        Vector3 target = waypoints[waypointIndex];
        Vector3 flatPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(target.x, 0f, target.z);

        float horizontalDistance = Vector3.Distance(flatPos, flatTarget);

        // Trigger a jump just before stepping onto a noticeably higher cell.
        if (grounded && !hasJumpedForCurrentSegment && horizontalDistance <= jumpTriggerDistance)
        {
            float heightDiff = target.y - transform.position.y;
            if (heightDiff > jumpStepThreshold)
            {
                verticalVelocity = jumpSpeed;
                grounded = false;
                hasJumpedForCurrentSegment = true;
            }
        }

        Vector3 direction = flatTarget - flatPos;
        if (direction.sqrMagnitude > 0.0001f)
        {
            direction.Normalize();
            Vector3 move = direction * moveSpeed * Time.deltaTime;

            if (move.magnitude > horizontalDistance)
                move = direction * horizontalDistance;

            transform.position += move;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        if (horizontalDistance <= arrivalThreshold && grounded)
        {
            AdvanceWaypoint();
        }
    }

    private void AdvanceWaypoint()
    {
        waypointIndex++;
        hasJumpedForCurrentSegment = false;

        if (waypointIndex >= waypoints.Count)
            OnTargetReached?.Invoke();
    }

    private void ApplyGravityAndGround()
    {
        int gx = Mathf.RoundToInt(transform.position.x);
        int gz = Mathf.RoundToInt(transform.position.z);
        float groundY = grid.GetSurfaceHeight(gx, gz) + 1f;

        Vector3 pos = transform.position;

        if (pos.y > groundY + groundedSnapThreshold)
        {
            // Airborne - apply gravity.
            verticalVelocity += gravity * Time.deltaTime;
            pos.y += verticalVelocity * Time.deltaTime;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                verticalVelocity = 0f;
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }
        else
        {
            // On or below ground level for this column - snap onto it.
            pos.y = groundY;
            verticalVelocity = 0f;
            grounded = true;
        }

        transform.position = pos;
    }
}

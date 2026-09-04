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

    [Tooltip("How close (horizontally) to a target column before the jump is triggered.")]
    public float jumpTriggerDistance = 0.4f;

    public bool IsGrounded { get; private set; } = true;

    private TerrainGrid grid;
    private float verticalVelocity;
    private bool hasJumpedThisSegment;

    public void Initialize(TerrainGrid terrainGrid)
    {
        grid = terrainGrid;
        verticalVelocity = 0f;
        IsGrounded = true;
        hasJumpedThisSegment = false;
    }

    public void Warp(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        verticalVelocity = 0f;
        IsGrounded = true;
        hasJumpedThisSegment = false;
    }

    public void ResetSegment()
    {
        hasJumpedThisSegment = false;
    }

    public bool MoveToward(Vector3 target, bool allowJump)
    {
        Vector3 flatPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(target.x, 0f, target.z);
        float horizontalDistance = Vector3.Distance(flatPos, flatTarget);

        if (allowJump && IsGrounded && !hasJumpedThisSegment && horizontalDistance <= jumpTriggerDistance)
        {
            float targetGroundY = GetGroundY(target.x, target.z);
            float heightDiff = targetGroundY - transform.position.y;
            if (heightDiff > jumpStepThreshold)
            {
                verticalVelocity = jumpSpeed;
                IsGrounded = false;
                hasJumpedThisSegment = true;
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

        ApplyGravityAndGround();

        return horizontalDistance <= arrivalThreshold && IsGrounded;
    }

    public void HoldGround()
    {
        ApplyGravityAndGround();
    }

    private void ApplyGravityAndGround()
    {
        float groundY = GetGroundY(transform.position.x, transform.position.z);
        Vector3 pos = transform.position;

        if (pos.y > groundY + groundedSnapThreshold)
        {
            verticalVelocity += gravity * Time.deltaTime;
            pos.y += verticalVelocity * Time.deltaTime;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                verticalVelocity = 0f;
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }
        else
        {
            pos.y = groundY;
            verticalVelocity = 0f;
            IsGrounded = true;
        }

        transform.position = pos;
    }

    private float GetGroundY(float worldX, float worldZ)
    {
        int gx = Mathf.RoundToInt(worldX);
        int gz = Mathf.RoundToInt(worldZ);
        return grid.GetSurfaceHeight(gx, gz) + 1f;
    }
}

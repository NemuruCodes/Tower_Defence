using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=jnETyJUiCiM


public enum EnemyState
{
    PathMoving,
    TowerMoving,
    TowerAttacking,
    PathReturning,
    Dead
}

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyStateMachine : MonoBehaviour
{
    [Header("Tower Detection")]
    [Tooltip("How far along the path the enemy will notice a tower.")]
    public float towerDetectionRange = 6f;
    public LayerMask towerLayerMask;
    [Tooltip("How often (seconds) to scan for towers while on the path.")]
    public float detectionInterval = 0.25f;

    [Header("Tower Combat")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackInterval = 1f;

    public System.Action OnTargetReached;

    public System.Action OnDeath;

    public EnemyState State { get; private set; } = EnemyState.PathMoving;

    private EnemyMover mover;
    private EnemyHealth health;
    private TerrainGrid grid;

    private List<Vector3> pathWaypoints;
    private int waypointIndex;
    private Vector3 returnWaypoint;

    private IDamageable currentTarget;
    private MonoBehaviour currentTargetBehaviour;
    private float attackTimer;
    private float detectionTimer;

    private void Awake()
    {
        mover = GetComponent<EnemyMover>();
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    public void Initialize(TerrainGrid terrainGrid, List<Vector2Int> path)
    {
        grid = terrainGrid;
        mover.Initialize(terrainGrid);

        pathWaypoints = BuildWaypoints(path);
        waypointIndex = 0;
        currentTarget = null;
        currentTargetBehaviour = null;
        detectionTimer = 0f;
        State = EnemyState.PathMoving;

        if (pathWaypoints.Count > 0)
            mover.Warp(pathWaypoints[0]);
    }

    private List<Vector3> BuildWaypoints(List<Vector2Int> path)
    {
        var result = new List<Vector3>(path.Count);
        foreach (Vector2Int cell in path)
        {
            float y = grid.GetSurfaceHeight(cell.x, cell.y) + 1f;
            result.Add(new Vector3(cell.x, y, cell.y));
        }
        return result;
    }

    private void Update()
    {
        if (State == EnemyState.Dead)
            return;

        if (pathWaypoints == null || pathWaypoints.Count == 0)
            return;

        switch (State)
        {
            case EnemyState.PathMoving:
                TickPathMoving();
                break;
            case EnemyState.TowerMoving:
                TickTowerMoving();
                break;
            case EnemyState.TowerAttacking:
                TickTowerAttacking();
                break;
            case EnemyState.PathReturning:
                TickPathReturning();
                break;
        }
    }

    private void TickPathMoving()
    {
        detectionTimer -= Time.deltaTime;
        if (detectionTimer <= 0f)
        {
            detectionTimer = detectionInterval;

            if (TryFindTarget(out IDamageable target, out MonoBehaviour targetBehaviour))
            {
                Debug.Log($"[{name}] Found target: {targetBehaviour.name} - switching to TowerMoving", this);
                currentTarget = target;
                currentTargetBehaviour = targetBehaviour;
                mover.ResetSegment();
                State = EnemyState.TowerMoving;
                return;
            }
        }

        if (waypointIndex >= pathWaypoints.Count)
            return;

        bool arrived = mover.MoveToward(pathWaypoints[waypointIndex], true);
        if (arrived)
        {
            waypointIndex++;
            mover.ResetSegment();

            if (waypointIndex >= pathWaypoints.Count)
                OnTargetReached?.Invoke();
        }
    }

    private void TickTowerMoving()
    {
        if (!TargetStillValid())
        {
            EnterPathReturning();
            return;
        }

        Vector3 targetPos = currentTargetBehaviour.transform.position;
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance <= attackRange)
        {
            attackTimer = 0f;
            State = EnemyState.TowerAttacking;
            return;
        }

        mover.MoveToward(targetPos, true);
    }

    private void TickTowerAttacking()
    {
        if (!TargetStillValid())
        {
            EnterPathReturning();
            return;
        }

        Vector3 targetPos = currentTargetBehaviour.transform.position;
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > attackRange)
        {
            State = EnemyState.TowerMoving;
            return;
        }

        Vector3 lookDir = targetPos - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackInterval;
            currentTarget.TakeDamage(attackDamage);
        }

        mover.HoldGround();
    }

    private void EnterPathReturning()
    {
        // The  enimies will try get  back to thier  path but a  bit closer to where they are
        waypointIndex = FindClosestWaypointIndex();
        returnWaypoint = pathWaypoints[waypointIndex];
        mover.ResetSegment();
        State = EnemyState.PathReturning;
    }

    private int FindClosestWaypointIndex()
    {
        int bestIndex = 0;
        float bestDist = float.MaxValue;

        for (int i = 0; i < pathWaypoints.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, pathWaypoints[i]);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void TickPathReturning()
    {
        bool arrived = mover.MoveToward(returnWaypoint, true);
        if (arrived)
        {
            currentTarget = null;
            currentTargetBehaviour = null;
            detectionTimer = 0f; 
            State = EnemyState.PathMoving;
        }
    }


    private void HandleDeath()
    {
        State = EnemyState.Dead;
        currentTarget = null;
        currentTargetBehaviour = null;

        OnDeath?.Invoke();
    }

  
    private bool TryFindTarget(out IDamageable target, out MonoBehaviour targetBehaviour)
    {
        target = null;
        targetBehaviour = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, towerDetectionRange, towerLayerMask);

        Debug.Log($"[{name}] Detection scan: {hits.Length} collider(s) on layer mask {DescribeTowerLayerMask(towerLayerMask)} within {towerDetectionRange}m", this);

        if (hits.Length == 0)
            return false;

        MonoBehaviour closestBehaviour = null;
        IDamageable closestDamageable = null;
        float closestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            bool hasDamageable = damageable != null;
            Debug.Log($"[{name}]   hit: {hit.name} (layer {LayerMask.LayerToName(hit.gameObject.layer)}) - has IDamageable: {hasDamageable}" +
                      (hasDamageable ? $", CurrentHealth: {damageable.CurrentHealth}" : ""), hit);

            if (!hasDamageable || damageable.CurrentHealth <= 0f)
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestDamageable = damageable;
                closestBehaviour = damageable as MonoBehaviour;
            }
        }

        if (closestDamageable == null)
        {
            Debug.Log($"[{name}] No valid IDamageable target among the hits.", this);
            return false;
        }

        target = closestDamageable;
        targetBehaviour = closestBehaviour;
        return true;
    }

    private static string DescribeTowerLayerMask(LayerMask mask)
    {
        if (mask.value == 0)
            return "NOTHING (mask is empty - check the Inspector!)";

        var names = new System.Collections.Generic.List<string>();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                names.Add(string.IsNullOrEmpty(layerName) ? $"Layer{i}" : layerName);
            }
        }
        return names.Count > 0 ? string.Join(", ", names) : $"raw value {mask.value}";
    }

    private bool TargetStillValid()
    {
        if (currentTargetBehaviour == null || currentTarget == null || currentTarget.CurrentHealth <= 0f)
            return false;

        float dist = Vector3.Distance(transform.position, currentTargetBehaviour.transform.position);
        return dist <= towerDetectionRange * 1.5f; // buffer so it doesn't flicker at the edge
    }
}

using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=jnETyJUiCiM

public enum EnemyState { Moving, Attacking, Dead }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]

public class EnemyStateMachine : MonoBehaviour
{
    
        [Header("Detection")]
        [SerializeField] private float detectRange = 4f;
        [SerializeField] private LayerMask towerLayer;
        [SerializeField] private float detectionInterval = 0.25f; // check on a timer, not every frame

        [Header("Attack")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackInterval = 1f;

        private NavMeshAgent agent;
        private EnemyHealth health;
        private EnemyState currentState = EnemyState.Moving;

        private IDamageable currentTarget;
        private MonoBehaviour currentTargetBehaviour; // needed to check if the target object still exists
        private float detectionTimer;
        private float attackTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
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

        private void Update()
        {
            if (currentState == EnemyState.Dead) return;

            switch (currentState)
            {
                case EnemyState.Moving:
                    TickMoving();
                    break;
                case EnemyState.Attacking:
                    TickAttacking();
                    break;
            }
        }

        private void TickMoving()
        {
            detectionTimer -= Time.deltaTime;
            if (detectionTimer <= 0f)
            {
                detectionTimer = detectionInterval;
                TryFindTower();
            }
        }

        private void TickAttacking()
        {
            // target died or got out of range some other way go back to moving
            if (currentTargetBehaviour == null || !IsTargetStillValid())
            {
                ExitAttacking();
                return;
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = attackInterval;
                currentTarget.TakeDamage(attackDamage);
            }
        }

        private void TryFindTower()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, towerLayer);
            if (hits.Length == 0) return;

            // pick nearest tower found
            Collider nearest = hits[0];
            float nearestDist = Vector3.Distance(transform.position, nearest.transform.position);
            for (int i = 1; i < hits.Length; i++)
            {
                float d = Vector3.Distance(transform.position, hits[i].transform.position);
                if (d < nearestDist) { nearest = hits[i]; nearestDist = d; }
            }

            if (nearest.TryGetComponent<IDamageable>(out var damageable))
            {
                EnterAttacking(damageable, nearest.GetComponent<MonoBehaviour>());
            }
        }

        private bool IsTargetStillValid()
        {
            float dist = Vector3.Distance(transform.position, currentTargetBehaviour.transform.position);
            return dist <= detectRange * 1.1f; // small buffer so it doesn't flicker at the edge
        }

        private void EnterAttacking(IDamageable target, MonoBehaviour targetBehaviour)
        {
            currentTarget = target;
            currentTargetBehaviour = targetBehaviour;
            currentState = EnemyState.Attacking;
            attackTimer = 0f;
            agent.isStopped = true;

            if (targetBehaviour is Tower tower)
                tower.OnDeath += HandleTargetDeath;
        }

        private void HandleTargetDeath() => ExitAttacking();

        private void ExitAttacking()
        {
            currentTarget = null;
            currentTargetBehaviour = null;
            currentState = EnemyState.Moving;
            detectionTimer = 0f; // re-check immediately in case another tower is right there
            agent.isStopped = false;
        }

        private void HandleDeath()
        {
            currentState = EnemyState.Dead;
            agent.isStopped = true;
        }
    }

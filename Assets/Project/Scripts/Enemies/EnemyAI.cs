using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Wander, Chase, Attack }

    [SerializeField] private EnemyData data;
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float exitRangeMultiplier = 1.2f;
    [SerializeField] private float wanderSpeedRatio = 0.5f;
    [SerializeField] private LayerMask obstacleMask;
    private float lastSeenTime;
    private Vector3 lastKnownPosition;
    private NavMeshAgent agent;
    private Transform target;
    private State state;
    private Vector3 homePosition;
    private float lastRepathTime;
    private bool isWaiting;
    private float wanderResumeTime;
    
    private IDamageable targetDamageable;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        lastAttackTime = -999f;
        homePosition = transform.position;
        target = GameObject.FindWithTag("Player").transform;
        target.TryGetComponent(out targetDamageable);
        ChangeState(State.Wander);
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float sqrDistance = toTarget.sqrMagnitude;

        switch (state)
        {
            case State.Wander:
                UpdateWander(sqrDistance, toTarget);
                break;
            case State.Chase:
                UpdateChase(sqrDistance);
                break;
            case State.Attack:
                UpdateAttack(sqrDistance, toTarget);
                break;
        }
    }

    private void UpdateWander(float sqrDistance, Vector3 toTarget)
    {
        if (sqrDistance <= data.detectRange * data.detectRange && CanSee(toTarget))
        {
            ChangeState(State.Chase);
            return;
        }

        bool arrived = !agent.pathPending
                       && agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (!arrived)
        {
            isWaiting = false;
            return;
        }

        if (!isWaiting)
        {
            isWaiting = true;
            wanderResumeTime = Time.time + data.wanderWaitTime;
            return;
        }

        if (Time.time >= wanderResumeTime)
        {
            PickWanderPoint();
            isWaiting = false;
        }
    }

    private void PickWanderPoint()
    {
        Vector2 circle = Random.insideUnitCircle * data.wanderRadius;
        Vector3 candidate = homePosition + new Vector3(circle.x, 0f, circle.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void UpdateChase(float sqrDistance)
    {
        if (sqrDistance > data.loseRange * data.loseRange)
        {
            ChangeState(State.Wander);
            return;
        }

        bool seen = HasLineOfSight();

        if (seen)
        {
            lastSeenTime = Time.time;
            lastKnownPosition = target.position;
        }
        else if (Time.time - lastSeenTime >= data.loseSightTime)
        {
            Debug.Log($"{data.enemyName}: 플레이어를 놓쳐 추격 포기");
            ChangeState(State.Wander);
            return;
        }

        if (seen && sqrDistance <= data.attackRange * data.attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        if (Time.time - lastRepathTime < repathInterval) return;
        lastRepathTime = Time.time;
        agent.SetDestination(lastKnownPosition);
    }

    private void UpdateAttack(float sqrDistance, Vector3 toTarget)
    {
        float exitRange = data.attackRange * exitRangeMultiplier;

        if (sqrDistance > exitRange * exitRange || !HasLineOfSight())
        {
            ChangeState(State.Chase);
            return;
        }

        if (toTarget.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(toTarget);
        }
        if (data.keepDistance)
        {
            KeepDistance(sqrDistance, toTarget);
        }
        if (Time.time - lastAttackTime >= data.attackInterval)
        {
            lastAttackTime = Time.time;

            if (targetDamageable != null)
            {
                targetDamageable.TakeDamage(data.attackDamage);
            }
        }
    }

    private void ChangeState(State next)
    {
        state = next;
        if (state == State.Chase)
        {
            lastSeenTime = Time.time;
            lastKnownPosition = target.position;
        }
        Debug.Log($"{data.enemyName} 상태: {state}");

        agent.isStopped = (state == State.Attack);
        agent.speed = data.moveSpeed * (state == State.Wander ? wanderSpeedRatio : 1f);

        if (state == State.Wander)
        {
            isWaiting = false;
            PickWanderPoint();
        }
    }
    private void KeepDistance(float sqrDistance, Vector3 toTarget)
    {
        bool tooClose = sqrDistance < data.preferredRange * data.preferredRange;
        agent.isStopped = !tooClose;

        if (!tooClose) return;
        if (Time.time - lastRepathTime < repathInterval) return;
        lastRepathTime = Time.time;

        Vector3 away = -toTarget.normalized;
        Vector3 candidate = transform.position + away * data.retreatStep;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            float currentSqr = (transform.position - target.position).sqrMagnitude;
            float newSqr = (hit.position - target.position).sqrMagnitude;

            if (newSqr > currentSqr + 1f)
            {
                agent.SetDestination(hit.position);
                return;
            }
        }

        agent.ResetPath();
    }
    private bool HasLineOfSight()
    {
        Vector3 eye = transform.position + Vector3.up * 1f;
        Vector3 targetPoint = target.position + Vector3.up * 1f;

        return !Physics.Linecast(eye, targetPoint, obstacleMask);
    }
    private bool CanSee(Vector3 toTarget)
    {
        if (Vector3.Angle(transform.forward, toTarget) > data.viewAngle * 0.5f) return false;

        return HasLineOfSight();
    }
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        float half = data.viewAngle * 0.5f;
        Vector3 left = Quaternion.Euler(0f, -half, 0f) * transform.forward * data.detectRange;
        Vector3 right = Quaternion.Euler(0f, half, 0f) * transform.forward * data.detectRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);
    }
}
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
                UpdateWander(sqrDistance);
                break;
            case State.Chase:
                UpdateChase(sqrDistance);
                break;
            case State.Attack:
                UpdateAttack(sqrDistance, toTarget);
                break;
        }
    }

    private void UpdateWander(float sqrDistance)
    {
        if (sqrDistance <= data.detectRange * data.detectRange)
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

        if (sqrDistance <= data.attackRange * data.attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        if (Time.time - lastRepathTime < repathInterval) return;
        lastRepathTime = Time.time;
        agent.SetDestination(target.position);
    }

    private void UpdateAttack(float sqrDistance, Vector3 toTarget)
    {
        float exitRange = data.attackRange * exitRangeMultiplier;

        if (sqrDistance > exitRange * exitRange)
        {
            ChangeState(State.Chase);
            return;
        }

        if (toTarget.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(toTarget);
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
        Debug.Log($"{data.enemyName} 상태: {state}");

        agent.isStopped = (state == State.Attack);
        agent.speed = data.moveSpeed * (state == State.Wander ? wanderSpeedRatio : 1f);

        if (state == State.Wander)
        {
            isWaiting = false;
            PickWanderPoint();
        }
    }
}
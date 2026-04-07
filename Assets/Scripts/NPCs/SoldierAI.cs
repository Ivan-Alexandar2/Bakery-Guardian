using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : Unit
{
    protected enum State { Patrol, Chase, Attack }

    [Header("Combat References")]
    public AggroSensor sensor;

    [Header("Patrol Settings")]
    public Transform guardPoint; // Assign their Home Building (or a specific flag)
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;
    private float pathUpdateTimer = 0f; // Stops the NavMesh from crashing

    [Header("Debug")]
    [SerializeField] protected State currentState;
    [SerializeField] protected Transform currentTarget;
    protected float attackCooldown;
    protected float patrolTimer;

    protected override void Start()
    {
        base.Start();
        currentState = State.Patrol;

        // Safety: If no guard point assigned, guard current location
        if (guardPoint == null)
        {
            GameObject tempPoint = new GameObject("GuardPoint_" + gameObject.name);
            tempPoint.transform.position = transform.position;
            guardPoint = tempPoint.transform;
        }

        SetRandomPatrolPoint();
    }

    protected virtual void Update()
    {
        base.Update();

        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();
            if (currentTarget != null) currentState = State.Chase;
            else currentState = State.Patrol;
        }

        // If we are falling from the sky or dead, don't do any NavMesh math!
        if (!agent.isOnNavMesh) return;

        switch (currentState)
        {
            case State.Patrol:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    patrolTimer += Time.deltaTime;
                    if (patrolTimer >= patrolWaitTime)
                    {
                        SetRandomPatrolPoint();
                        patrolTimer = 0;
                    }
                }
                if (currentTarget != null) currentState = State.Chase;
                break;

            case State.Chase:
                if (currentTarget == null) { currentState = State.Patrol; break; }

                float distToTarget = Vector3.Distance(transform.position, currentTarget.position);

                if (distToTarget > sensor.detectionRange * 1.5f)
                {
                    currentTarget = null;
                    agent.ResetPath();
                    currentState = State.Patrol;
                    break;
                }

                pathUpdateTimer -= Time.deltaTime;
                if (pathUpdateTimer <= 0f)
                {
                    // Only ask for a path 5 times a second instead of 60!
                    agent.SetDestination(currentTarget.position);
                    pathUpdateTimer = 0.2f;
                }

                if (GetDistanceToTarget() <= stats.attackRange)
                {
                    currentState = State.Attack;
                    agent.ResetPath();
                }
                break;

            case State.Attack:
                if (currentTarget == null)
                {
                    currentState = State.Patrol;
                    break;
                }

                agent.ResetPath();
                FaceTarget();

                if (GetDistanceToTarget() > stats.attackRange + 0.5f)
                {
                    currentState = State.Chase;
                }

                if (attackCooldown <= 0 && IsAimingAtTarget())
                {
                    PerformAttack();
                }
                break;
        }
    }

    protected void SetRandomPatrolPoint()
    {
        // Get a random point inside a sphere
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        if(guardPoint != null) randomDirection += guardPoint.position; // null check cuz of the main bakery

        // Find the nearest valid spot on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    // "Virtual" means "Archers can change how this works"
    protected virtual void PerformAttack()
    {
        attackCooldown = stats.attackSpeed;

        // Get the enemy script (Using our universal Unit class)
        Unit enemy = currentTarget.GetComponentInParent<Unit>();
        if (enemy != null)
        {
            enemy.TakeDamage(stats.damage);
        }

        Building building = currentTarget.GetComponentInParent<Building>();
        if (building != null)
        {
            building.TakeDamage(stats.damage);
        }
    }

    protected float GetDistanceToTarget()
    {
        if (currentTarget == null) return 999f;

        // 1. Try to find a collider on the target (or its children!)
        // We use GetComponentInChildren because your Root object might not have a collider,
        // but the "Body" child definitely does.
        Collider targetCol = currentTarget.GetComponentInChildren<Collider>();

        if (targetCol != null)
        {
            // Smart Math: Distance to the closest point on the surface
            Vector3 closestPoint = targetCol.ClosestPoint(transform.position);
            return Vector3.Distance(transform.position, closestPoint);
        }
        else
        {
            // Dumb Math: Fallback to center
            return Vector3.Distance(transform.position, currentTarget.position);
        }
    }

    protected virtual void FaceTarget()
    {
        if (currentTarget != null) transform.LookAt(currentTarget);
    }

    // Virtual means standard soldiers always say "Yes", but tanks can say "No, I'm not aimed yet!"
    protected virtual bool IsAimingAtTarget()
    {
        return true;
    }
}

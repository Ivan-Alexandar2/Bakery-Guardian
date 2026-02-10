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

    [Header("Debug")]
    [SerializeField] protected State currentState;
    [SerializeField] protected Transform currentTarget;
    protected float attackCooldown;
    private float patrolTimer;

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
        // 1. Update Cooldowns
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        // 2. Always look for a target if we don't have one
        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();
            if (currentTarget != null) currentState = State.Chase;
            else currentState = State.Patrol;
        }

        // 3. State Machine
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
                if (currentTarget == null)
                {
                    currentState = State.Patrol;
                    SetRandomPatrolPoint();
                    break;
                }

                // MOVEMENT
                agent.SetDestination(currentTarget.position);

                // DECISION: Use the Universal Ruler
                if (GetDistanceToTarget() <= stats.attackRange)
                {
                    currentState = State.Attack;
                    agent.ResetPath(); // Stop moving instantly
                }
                break;

            case State.Attack:
                if (currentTarget == null)
                {
                    currentState = State.Patrol;
                    break;
                }

                agent.ResetPath();
                transform.LookAt(currentTarget);

                // DECISION: Use the SAME Universal Ruler
                // Only chase if we are genuinely out of range
                // We add a tiny buffer (+ 0.5f) to prevent micro-twitching at the edge
                if (GetDistanceToTarget() > stats.attackRange + 0.5f)
                {
                    currentState = State.Chase;
                }

                if (attackCooldown <= 0) PerformAttack();
                break;
        }
    }

    void SetRandomPatrolPoint()
    {
        // Get a random point inside a sphere
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += guardPoint.position;

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
        // Default Melee Logic:
        // 1. Reset Cooldown (attackCooldown = stats.attackSpeed)
        // 2. Get Unit component from target
        // 3. Deal Damage
        attackCooldown = stats.attackSpeed;

        // Get the enemy script (Using our universal Unit class)
        Unit enemy = currentTarget.GetComponentInParent<Unit>();
        if (enemy != null)
        {
            enemy.TakeDamage(stats.damage);
            Debug.Log(name + " hit " + enemy.name + " for " + stats.damage);
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
}

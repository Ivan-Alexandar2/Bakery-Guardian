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
                if (currentTarget == null) { currentState = State.Patrol; break; }

                // --- NEW: THE LEASH (GIVE UP LOGIC) ---
                float distToTarget = Vector3.Distance(transform.position, currentTarget.position);

                // If the target ran too far away (e.g., 20 meters), give up.
                // Or if WE ran too far from our guard post (e.g., 30 meters).
                if (distToTarget > sensor.detectionRange * 1.5f)
                {
                    Debug.Log("Target escaped! Returning to post.");
                    currentTarget = null;
                    agent.ResetPath();
                    currentState = State.Patrol; // Or ReturnToGuardPoint
                    break;
                }

                agent.SetDestination(currentTarget.position);

                // (Your existing attack range check here...)
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

                // 1. USE AIMING METHOD
                FaceTarget();

                if (GetDistanceToTarget() > stats.attackRange + 0.5f)
                {
                    currentState = State.Chase;
                }

                // 2. CHECK IF WE ARE AIMED BEFORE SHOOTING
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

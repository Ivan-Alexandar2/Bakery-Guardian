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
                // Hints:
                // Check if target is null (died?). If so, go back to Idle.
                // Move towards target using agent.SetDestination.
                // Check Distance: if (distance <= stats.attackRange) -> Switch to Attack

                if (currentTarget == null)
                {
                    currentState = State.Patrol;
                    SetRandomPatrolPoint();
                    break;
                }
                
                agent.SetDestination(currentTarget.position);

                if(Vector3.Distance(transform.position, currentTarget.position) <= stats.attackRange) currentState = State.Attack;
                
                break;

            case State.Attack:
                // Hints:
                // Stop moving (agent.ResetPath).
                // Look at enemy (transform.LookAt).
                // Check Distance: if (distance > stats.attackRange) -> Switch to Chase (He ran away!)
                // If (attackCooldown <= 0) -> PerformAttack();

                if (currentTarget == null)
                {
                    currentState = State.Patrol;
                    break;
                }

                agent.ResetPath();
                transform.LookAt(currentTarget);
                if (Vector3.Distance(transform.position, currentTarget.position) > stats.attackRange) currentState = State.Chase;
                if(attackCooldown <= 0) PerformAttack();

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
        Unit enemy = currentTarget.GetComponent<Unit>();
        if (enemy != null)
        {
            enemy.TakeDamage(stats.damage);
            Debug.Log(name + " hit " + enemy.name + " for " + stats.damage);
        }
    }
}

using UnityEngine;

public class HealerAI : SoldierAI
{
    [Header("Healer Settings")]
    public float healAmount = 10f;
    public float healRate = 2f;
    public float fleeDistance = 15f; // Run 15m away

    [Header("Sensors")]
    public HealerSensor patientSensor; // Finds friends
    public AggroSensor dangerSensor;   // Finds enemies (reuse existing script!)


    // OVERRIDE Start to prevent SoldierAI from messing with targeting
    protected override void Start()
    {
        base.Start();
        // Ensure danger sensor only looks for enemies (Layer setup check)
    }
    protected override void Update()
    {
        // 1. SELF PRESERVATION (Priority #1)
        // Look for enemies independently of 'currentTarget'
        Transform threat = dangerSensor.GetTarget();

        // If threat exists and is close (e.g. 10m) -> RUN
        if (threat != null && Vector3.Distance(transform.position, threat.position) < 10f)
        {
            FleeFrom(threat.position);
            return; // Stop here, do not heal, do not patrol
        }

        // 2. FIND PATIENT (Priority #2)
        if (currentTarget == null)
        {
            // Only look for PATIENTS, never enemies
            currentTarget = patientSensor.GetInjuredAlly();
        }

        // 3. DO DOCTOR STUFF
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            if (dist <= stats.attackRange)
            {
                // In range -> Heal
                agent.ResetPath();
                if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

                if (attackCooldown <= 0) PerformHeal();
            }
            else
            {
                // Out of range -> Chase Patient
                agent.SetDestination(currentTarget.position);
            }
        }
        else
        {
            // 4. NOTHING TO DO -> IDLE / PATROL
            // Only patrol if safe
            PatrolLogic();
        }

        // 4. STATE MACHINE
        switch (currentState)
        {
            case State.Patrol:
                // Normal patrol (from SoldierAI), but keep checking for patients
                base.Update();
                if (patientSensor.GetInjuredAlly() != null) currentState = State.Chase;
                break;

            case State.Chase:
                if (currentTarget == null) { currentState = State.Patrol; break; }

                agent.SetDestination(currentTarget.position);

                // If close enough to touch -> Heal
                if (Vector3.Distance(transform.position, currentTarget.position) <= stats.attackRange)
                {
                    currentState = State.Attack; // "Attack" means "Heal" for us
                    agent.ResetPath();
                }
                break;

            case State.Attack:
                if (currentTarget == null) { currentState = State.Patrol; break; }

                // If patient walked away, chase him
                if (Vector3.Distance(transform.position, currentTarget.position) > stats.attackRange)
                {
                    currentState = State.Chase;
                }

                if (attackCooldown <= 0) PerformHeal();
                break;
        }
    }

    void PatrolLogic()
    {
        // Copy-paste the random movement logic here, 
        // OR make the Patrol logic in SoldierAI 'protected' so you can call it.
        // For simplicity, just make them stand still or wander near GuardPoint.
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                SetRandomPatrolPoint();
                patrolTimer = 0;
            }
        }
    }

    protected void FleeFrom(Vector3 enemyPos)
    {
        // Calculate direction AWAY from enemy
        Vector3 fleeDir = (transform.position - enemyPos).normalized;
        Vector3 fleePos = transform.position + fleeDir * fleeDistance;

        // Run there
        agent.SetDestination(fleePos);
        currentState = State.Patrol; // Reset state
    }

    protected virtual void PerformHeal()
    {
        attackCooldown = healRate;

        // Look for Unit script
        Unit patient = currentTarget.GetComponent<Unit>();
        if (patient != null)
        {
            // You need to add a Heal() method to Unit.cs!
            patient.Heal(healAmount);
            patient.CurePlague();
            // Visual Effect
            Debug.Log("<color=green>Healing + " + healAmount + "</color>");

            // If fully healed, stop targeting him
            if (patient.currentHealth >= patient.stats.maxHealth)
            {
                currentTarget = null;
                currentState = State.Patrol;
            }
        }
    }
}

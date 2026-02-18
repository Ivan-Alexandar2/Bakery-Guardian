using UnityEngine;

public class EnemyHealerAI : HealerAI
{
    [Header("Enemy Specifics")]
    public GameObject healingProjectilePrefab;
    public Transform firePoint;
    public float followDistance = 5f;
    public float dangerTriggerDist;
    private float lastTargetCheckTime;

    protected override void Start()
    {
        base.Start();
        stats.attackRange = 10f;

        // 1. MARCHING ORDER: Find the Player's Base
        GameObject mainBase = GameObject.FindGameObjectWithTag("MainBase");
        if (mainBase != null)
        {
            guardPoint = mainBase.transform;
        }
    }

    protected override void Update()
    {
        // 1. SELF PRESERVATION (Priority #1)
        Transform threat = dangerSensor.GetTarget();
        if (threat != null && Vector3.Distance(transform.position, threat.position) < dangerTriggerDist)
        {
            FleeFrom(threat.position);
            return;
        }

        // 2. TARGET SELECTION (Priority #2)
        if (Time.time > lastTargetCheckTime + 0.5f)
        {
            lastTargetCheckTime = Time.time;

            // A. Injured?
            Transform injured = patientSensor.GetInjuredAlly();
            if (injured != null)
            {
                currentTarget = injured;
            }
            // B. Healthy Friend?
            else
            {
                currentTarget = patientSensor.GetClosestAlly();
            }
        }

        // 3. MOVEMENT & ACTION
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            Unit patientUnit = currentTarget.GetComponent<Unit>();
            bool isHurt = patientUnit != null && patientUnit.currentHealth < patientUnit.stats.maxHealth;

            // COMBAT MEDIC MODE (Heal or Follow)
            if (isHurt && dist <= stats.attackRange)
            {
                agent.ResetPath();
                transform.LookAt(currentTarget);

                if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0) PerformHeal();
            }
            else
            {
                // Follow the Leader
                if (dist > followDistance)
                {
                    agent.SetDestination(currentTarget.position);
                }
                else
                {
                    agent.ResetPath();
                }
            }
        }
        else
        {
            // 4. NO FRIENDS? MARCH TO WAR!
            // Previously we had 'agent.ResetPath()' here, which made them stand still.
            // Now, we tell them to go to the Bakery.
            if (guardPoint != null)
            {
                agent.SetDestination(guardPoint.position);
            }
        }
    }

    protected override void PerformHeal()
    {
        attackCooldown = healRate;

        if (healingProjectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(healingProjectilePrefab, firePoint.position, transform.rotation);
            HealingSphere sphere = proj.GetComponent<HealingSphere>();
            // Setup the projectile to ignore "Enemy" layer and heal "Enemy" layer
            // (Remember: Sensor Target Layer for Enemy Healer is "Enemy")
            Collider myCollider = GetComponent<Collider>();
            sphere.Setup(sensor.targetLayer, myCollider);
        }
    }
}
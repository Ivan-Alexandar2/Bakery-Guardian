using UnityEngine;

public class EnemyHealerAI : HealerAI
{
    [Header("Enemy Specifics")]
    public GameObject healingProjectilePrefab;
    public Transform firePoint;
    public float followDistance = 5f; // How far to stay behind the leader
    private float lastTargetCheckTime;

    protected override void Start()
    {
        base.Start();
        stats.attackRange = 10f; // Throw potions from far away
    }

    // THIS OVERRIDE ENSURES ONLY ENEMIES DO THIS BEHAVIOR
    protected override void Update()
    {
        // 1. SELF PRESERVATION (Priority #1 - Same as base)
        Transform threat = dangerSensor.GetTarget();
        if (threat != null && Vector3.Distance(transform.position, threat.position) < 10f) //danger distance
        {
            FleeFrom(threat.position);
            return;
        }

        // 2. TARGET SELECTION (Priority #2)
        // Check every 0.5s for a target (Injured OR Healthy)
        if (Time.time > lastTargetCheckTime + 0.5f)
        {
            lastTargetCheckTime = Time.time;

            // A. Is anyone hurt? (Highest Priority)
            Transform injured = patientSensor.GetInjuredAlly();

            if (injured != null)
            {
                currentTarget = injured;
            }
            // B. If nobody is hurt, find the CLOSEST friend to follow (Bodyguard Mode)
            else
            {
                currentTarget = patientSensor.GetClosestAlly();
            }
        }

        // 3. MOVEMENT & ACTION
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            // LOGIC A: HEALING
            // Check if the target is actually hurt
            Unit patientUnit = currentTarget.GetComponent<Unit>();
            bool isHurt = patientUnit != null && patientUnit.currentHealth < patientUnit.stats.maxHealth;

            // If hurt AND in range -> Heal (Throw Potion)
            if (isHurt && dist <= stats.attackRange)
            {
                agent.ResetPath();

                // Rotate to face them
                transform.LookAt(currentTarget);

                if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0) PerformHeal();
            }
            // LOGIC B: FOLLOWING
            // If healthy OR out of range -> Follow
            else
            {
                // Only move if we are too far away (maintain formation)
                if (dist > followDistance)
                {
                    agent.SetDestination(currentTarget.position);
                }
                else
                {
                    agent.ResetPath(); // Stop and wait
                }
            }
        }
        else
        {
            // 4. NO FRIENDS? IDLE.
            // Unlike Friendly Healer, we do NOT patrol. We wait for reinforcements.
            agent.ResetPath();
        }
    }

    protected override void PerformHeal()
    {
        attackCooldown = healRate;

        if (healingProjectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(healingProjectilePrefab, firePoint.position, transform.rotation);
            // Setup the projectile to ignore "Enemy" layer (which is us) and heal "Enemy" layer
            proj.GetComponent<HealingSphere>().Setup(sensor.targetLayer);
        }
    }
}
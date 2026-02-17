using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [Header("Setup")]
    public AggroSensor sensor;
    public Projectile projectilePrefab;
    public Transform firePoint;
    public Transform rotatingPart;

    [Header("Stats")]
    public float damage = 25f;
    public float fireRate = 2f;
    public float projectileSpeed = 30f;

    private Transform currentTarget;
    private float attackCooldown;

    void Update()
    {
        // ---------------------------------------------------------
        // YOUR CODE GOES HERE
        // ---------------------------------------------------------

        // 1. Target Logic:
        //    If 'currentTarget' is null, ask 'sensor.GetTarget()'.

        // 2. Combat Logic (Only if we have a target):
        //    a. Rotate towards target. 
        //       (Simple: transform.LookAt(currentTarget))
        //       (Pro: rotatingPart.LookAt(currentTarget) if you have a turret head)

        //    b. Cooldown Timer: Subtract Time.deltaTime.

        //    c. Fire: If cooldown <= 0:
        //       - Instantiate projectile.
        //       - Setup projectile (pass 'damage' and 'sensor.targetLayer').
        //       - Reset cooldown.

        //if (currentTarget == null) sensor.GetTarget();

        //if(currentTarget != null)
        //{
        //    rotatingPart.LookAt(currentTarget);
        //    attackCooldown -= Time.deltaTime;

        //    if (attackCooldown <= 0)
        //    {
        //        Instantiate(projectilePrefab);
        //        projectilePrefab.Setup(damage, sensor.targetLayer, projectileSpeed);
        //        attackCooldown = fireRate;
        //    }
        //}
        
        

        // 1. Assign the target
        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();
            rotatingPart.transform.rotation = Quaternion.identity;
        }

        // 2. Only run combat if we HAVE a target
        if (currentTarget != null)
        {
            // Safety: LookAt the target
            if (rotatingPart != null)
                rotatingPart.LookAt(currentTarget);
            else
                transform.LookAt(currentTarget);

            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0)
            {
                // Fix: Capture the clone + Spawn at FirePoint
                Projectile clone = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                // Fix: Setup the clone
                clone.Setup(damage, sensor.targetLayer, projectileSpeed);

                attackCooldown = fireRate;
            }
        }
    }
}

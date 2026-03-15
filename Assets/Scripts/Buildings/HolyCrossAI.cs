using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HolyCrossAI : MonoBehaviour // Inheriting from your existing tower logic
{
    [Header("Laser Settings")]
    public float damagePerTick = 5f; // How much damage it does per hit
    public float tickRate = 0.2f;    // How often it hits (0.2s = 5 times per second)  
    public AggroSensor sensor;
    public Transform firePoint;

    private Transform currentTarget;
    private LineRenderer laser;
    private float damageTimer = 0f;

    void Start()
    {
        laser = GetComponent<LineRenderer>();
        laser.enabled = false; // Keep the laser turned off until we have a target
    }

    void Update()
    {
        if(currentTarget ==null) currentTarget = sensor.GetTarget();
        if (currentTarget != null)
        {
            FireLaser();
        }
        else
        {
            StopLaser();
        }
    }

    void FireLaser()
    {
        laser.enabled = true;
        //  Connect Point A to Point B every single frame (so it tracks them as they move!)
        laser.SetPosition(0, firePoint.position);

        // We add Vector3.up so we shoot them in the chest, not the toes!
        laser.SetPosition(1, currentTarget.position + Vector3.up * 1f);

        // Damage Over Time (DoT) Logic
        damageTimer += Time.deltaTime;

        if (damageTimer >= tickRate)
        {
            // Time to burn them!
            Unit enemy = currentTarget.GetComponent<Unit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerTick);
            }

            // Reset the timer for the next tick
            damageTimer = 0f;
        }
    }

    void StopLaser()
    {
        laser.enabled = false;
        damageTimer = 0f;
    }
}
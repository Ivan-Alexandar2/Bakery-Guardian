using UnityEngine;
using UnityEngine.InputSystem;

public class MortarAI : TowerAI
{
    // Make sure you drag the MortarShell prefab here, not the standard Projectile
    public MortarShell mortarShellPrefab;

    protected override void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();
            if (rotatingPart != null) rotatingPart.transform.rotation = Quaternion.identity;
        }

        if (currentTarget != null)
        {
            // Note: Mortars usually don't need to look up/down, just left/right. 
            // You might want to lock the rotatingPart's Y-axis here if it looks goofy!
            if (rotatingPart != null) rotatingPart.LookAt(currentTarget);
            else transform.LookAt(currentTarget);

            // Access the private cooldown via Reflection, or (better) change 'private float attackCooldown' 
            // to 'protected float attackCooldown' in your base TowerAI.cs so we can use it here!
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0)
            {
                // Spawn the shell
                MortarShell clone = Instantiate(mortarShellPrefab, firePoint.position, firePoint.rotation);

                // LAUNCH IT at their feet!
                clone.Launch(firePoint.position, currentTarget.position, damage, sensor.targetLayer);

                attackCooldown = fireRate;
            }
        }
    }
}

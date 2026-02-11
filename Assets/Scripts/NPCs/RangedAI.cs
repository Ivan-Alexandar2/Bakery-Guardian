using UnityEngine;

public class RangedAI : SoldierAI
{
    [Header("Ranged Settings")]
    public Projectile projectilePrefab;
    public Transform firePoint;

    private void Start()
    {
        base.Start();
    }

    protected override void PerformAttack()
    {
        // Reset the cooldown (inherited from SoldierAI)
        attackCooldown = stats.attackSpeed;

        // 2. Get the 'Projectile' script from the new object.

        // 3. Call 'Setup()'. 
        //    Pass: stats.damage, sensor.targetLayer, and maybe 20f for speed.

        Projectile clone = Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        clone.Setup(stats.damage, sensor.targetLayer, 20f);
    }
}

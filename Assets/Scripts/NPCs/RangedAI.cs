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
        attackCooldown = stats.attackSpeed;

        //Pass: stats.damage, sensor.targetLayer, and 20f for speed.

        Projectile clone = Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        clone.Setup(stats.damage, sensor.targetLayer, clone.speed);
    }
}

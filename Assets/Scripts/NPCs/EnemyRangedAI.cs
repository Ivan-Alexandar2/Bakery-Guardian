using UnityEngine;

public class EnemyRangedAI : RangedAI
{
    [Header("Aim Settings")]
    public float turnSpeed = 2f;
    public float aimTolerance = 5f;

    protected override void Start()
    {
        base.Start();
        agent.updateRotation = false;

        GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
        if (baseObj != null) guardPoint = baseObj.transform;

        patrolRadius = 5f;
        patrolWaitTime = 0.5f;
    }

    protected override void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0;

        if (dirToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    protected override bool IsAimingAtTarget()
    {
        if (currentTarget == null) return false;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0;

        return Vector3.Angle(transform.forward, dirToTarget) <= aimTolerance;
    }

    protected override void PerformAttack()
    {
        attackCooldown = stats.attackSpeed;

        // Aim directly at the target's position at fire time, ignoring body rotation drift
        Vector3 dirToTarget = (currentTarget.position - firePoint.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(dirToTarget);

        Projectile clone = Instantiate(projectilePrefab, firePoint.position, aimRotation);
        clone.Setup(stats.damage, sensor.targetLayer, clone.speed);
    }
}
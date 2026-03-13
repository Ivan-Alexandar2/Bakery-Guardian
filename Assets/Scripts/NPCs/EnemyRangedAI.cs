using UnityEngine;

public class EnemyRangedAI : RangedAI // Inherits Shooting & Soldier logic
{
    [Header("Aim Settings")]
    public float turnSpeed = 2f; // Heavy/slow turning
    public float aimTolerance = 5f;

    protected override void Start()
    {
        base.Start();

        GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
        if (baseObj != null) guardPoint = baseObj.transform;

        patrolRadius = 5f;
        patrolWaitTime = 0.5f;
    }

    // Override the instant snap with our slow tank treads!
    protected override void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0; // Keep it flat

        if (dirToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    // Tell the State Machine we refuse to fire until the barrel is lined up
    protected override bool IsAimingAtTarget()
    {
        if (currentTarget == null) return false;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0;

        float angle = Vector3.Angle(transform.forward, dirToTarget);

        return angle <= aimTolerance;
    }

    // Notice we DON'T override PerformAttack() here! 
    // We let RangedAI handle spawning the cannonball!
}
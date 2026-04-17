using UnityEngine;
using UnityEngine.AI;

public class MortarTowerAI : TowerAI
{
    [Header("Mortar Mechanics")]
    public MortarShell mortarShellPrefab;
    public Transform mortarBase;
    public Transform mortarBarrel;

    [Header("Elevation Settings")]
    public float minElevation = 30f;
    public float maxElevation = 75f;
    public float turnSpeed = 5f;

    protected override void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();

            if (mortarBase != null && mortarBarrel != null)
            {
                mortarBase.localRotation = Quaternion.Slerp(mortarBase.localRotation, Quaternion.identity, Time.deltaTime * turnSpeed);
                mortarBarrel.localRotation = Quaternion.Slerp(mortarBarrel.localRotation, Quaternion.Euler(-45f, 0, 0), Time.deltaTime * turnSpeed);
            }
        }

        if (currentTarget != null)
        {
            AimMortar();

            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0)
            {
                Vector3 fireTarget = PredictTargetPosition();

                MortarShell clone = Instantiate(mortarShellPrefab, firePoint.position, firePoint.rotation);
                clone.Launch(firePoint.position, fireTarget, damage, sensor.targetLayer);

                attackCooldown = fireRate;
            }
        }
    }

    // Predicts where a moving target will be when the shell lands.
    // Falls back to current position if the target is standing still.
    Vector3 PredictTargetPosition()
    {
        NavMeshAgent targetAgent = currentTarget.GetComponentInParent<NavMeshAgent>();

        if (targetAgent != null && targetAgent.velocity.magnitude > 0.5f)
        {
            return currentTarget.position + targetAgent.velocity * mortarShellPrefab.travelTime;
        }

        return currentTarget.position;
    }

    void AimMortar()
    {
        // 1. SPIN THE BASE (Left / Right)
        Vector3 dirToBase = currentTarget.position - mortarBase.position;
        dirToBase.y = 0;

        if (dirToBase != Vector3.zero)
        {
            Quaternion targetBaseRot = Quaternion.LookRotation(dirToBase);
            mortarBase.rotation = Quaternion.Slerp(mortarBase.rotation, targetBaseRot, Time.deltaTime * turnSpeed);
        }

        // 2. TILT THE BARREL (Up / Down)
        float distance = Vector3.Distance(mortarBase.position, currentTarget.position);
        float distanceRatio = Mathf.Clamp01(distance / sensor.detectionRange);
        float targetPitch = Mathf.Lerp(maxElevation, minElevation, distanceRatio);

        Quaternion targetBarrelRot = Quaternion.Euler(targetPitch, 0f, 0f);
        if (mortarBarrel != null)
            mortarBarrel.localRotation = Quaternion.Slerp(mortarBarrel.localRotation, targetBarrelRot, Time.deltaTime * turnSpeed);
    }
}
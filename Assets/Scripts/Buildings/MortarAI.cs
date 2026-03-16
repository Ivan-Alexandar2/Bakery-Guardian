using UnityEngine;

public class MortarTowerAI : TowerAI
{
    [Header("Mortar Mechanics")]
    public MortarShell mortarShellPrefab;
    public Transform mortarBase;   // Assign the rotating base
    public Transform mortarBarrel; // Assign the tilting barrel/arm

    [Header("Elevation Settings")]
    public float minElevation = 30f; // Angle for targets at the very edge of your range
    public float maxElevation = 75f; // Angle for targets standing right next to the tower
    public float turnSpeed = 5f;     // How smoothly the mechanical parts move

    protected override void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = sensor.GetTarget();

            // Optional: Slowly return to a resting position when no enemies are around
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
                // Spawn and Launch!
                MortarShell clone = Instantiate(mortarShellPrefab, firePoint.position, firePoint.rotation);
                clone.Launch(firePoint.position, currentTarget.position, damage, sensor.targetLayer);

                attackCooldown = fireRate;
            }
        }
    }

    void AimMortar()
    {
        // 1. SPIN THE BASE (Left / Right)
        Vector3 dirToBase = currentTarget.position - mortarBase.position;
        dirToBase.y = 0; // Flatten it so the base doesn't tilt!

        if (dirToBase != Vector3.zero)
        {
            Quaternion targetBaseRot = Quaternion.LookRotation(dirToBase);
            mortarBase.rotation = Quaternion.Slerp(mortarBase.rotation, targetBaseRot, Time.deltaTime * turnSpeed);
        }

        // 2. TILT THE BARREL (Up / Down)
        // Check how far away the enemy is
        float distance = Vector3.Distance(mortarBase.position, currentTarget.position);

        // Convert that distance to a percentage (0.0 to 1.0) based on your sensor's max range
        float distanceRatio = Mathf.Clamp01(distance / sensor.detectionRange);

        // Calculate the perfect pitch. 
        // If they are far (ratio = 1), pitch is minElevation. If they are close (ratio = 0), pitch is maxElevation.
        float targetPitch = Mathf.Lerp(maxElevation, minElevation, distanceRatio);

        // Apply it as a Local Rotation. (In Unity, tilting UP is usually a negative X rotation)
        Quaternion targetBarrelRot = Quaternion.Euler(targetPitch, 0f, 0f);
        if(mortarBarrel != null)
            mortarBarrel.localRotation = Quaternion.Slerp(mortarBarrel.localRotation, targetBarrelRot, Time.deltaTime * turnSpeed);
    }
}
using UnityEditor.UIElements;
using UnityEngine;

public class AssassinSensor : AggroSensor
{
    protected override void ScanForEnemies()
    {
        // 1. Scan ONCE
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        // 2. Prepare our two buckets
        Transform bestHighPriority = null;
        float closestHighDist = Mathf.Infinity;

        Transform bestLowPriority = null;
        float closestLowDist = Mathf.Infinity;

        // 3. One single loop through the enemies
        foreach (Collider col in enemies)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);

            // 4. Identify the target type
            bool isHighPriority = col.GetComponent<RangedAI>() != null || col.GetComponent<HealerAI>() != null;

            // 5. Sort them into the correct bucket based on distance
            if (isHighPriority)
            {
                if (dist < closestHighDist)
                {
                    closestHighDist = dist;
                    bestHighPriority = col.transform;
                }
            }
            else
            {
                // It's a normal soldier or worker
                if (dist < closestLowDist)
                {
                    closestLowDist = dist;
                    bestLowPriority = col.transform;
                }
            }
        }

        // 6. THE ASSASSIN'S DECISION
        // Always pick the high priority target if one exists. 
        // Otherwise, fall back to the normal target.
        if (bestHighPriority != null)
        {
            currentTarget = bestHighPriority;
        }
        else
        {
            currentTarget = bestLowPriority;
        }
    }
}

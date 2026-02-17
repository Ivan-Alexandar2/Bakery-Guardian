using UnityEngine;

public class HealerSensor : AggroSensor // Reuse the range/timer logic!
{
    // Override the scan to find INJURED ALLIES
    public Transform GetInjuredAlly()
    {
        Collider[] potentialPatients = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        Transform bestPatient = null;
        float lowestHealthPercentage = 1.0f; // 100%

        foreach (Collider col in potentialPatients)
        {
            Unit patient = col.GetComponent<Unit>();
            if (patient != null && patient != this.GetComponent<Unit>()) // Don't heal self
            {
                // Calculate Health %
                float healthPct = patient.currentHealth / patient.stats.maxHealth;

                // Is he hurt? And is he the MOST hurt one we found?
                if (healthPct < 1.0f && healthPct < lowestHealthPercentage)
                {
                    lowestHealthPercentage = healthPct;
                    bestPatient = patient.transform;
                }
            }
        }
        return bestPatient;
    }

    public Transform GetClosestAlly()
    {
        Collider[] friends = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        Transform bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in friends)
        {
            // FIX 1: Is this ME? If so, skip.
            if (col.gameObject == this.gameObject) continue;

            // FIX 2: Is this part of my own hierarchy? (e.g. my own sensor child object)
            // This prevents detecting your own child colliders.
            if (col.transform.root == transform.root) continue;

            float d = Vector3.Distance(transform.position, col.transform.position);

            if (d < closestDist)
            {
                closestDist = d;
                bestTarget = col.transform;
            }
        }
        return bestTarget;
    }
}
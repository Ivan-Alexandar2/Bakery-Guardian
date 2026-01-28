using UnityEngine;

public class AggroSensor : MonoBehaviour
{
    public float detectionRange = 10f;
    public LayerMask targetLayer; // Set this to "Enemy" in Inspector
    public float checkRate = 0.5f; // Optimization: Don't check every frame!

    [SerializeField] private Transform currentTarget;
    [SerializeField] private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkRate)
        {
            timer = 0;
            ScanForEnemies();
        }
    }

    void ScanForEnemies()
    {
        // Physics check
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider enemy in enemies)
        {
            // Here is where "Rogue" logic would go later (checking enemy health etc)
            // For now, simple distance check:
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            // Check if the target is alive (Optional optimization) I think?!?
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = enemy.transform;
            }
        }

        currentTarget = bestTarget;
    }

    public Transform GetTarget() => currentTarget;

    // Visualize the range in Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

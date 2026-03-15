using UnityEngine;

public class MortarShell : MonoBehaviour
{
    [Header("Trajectory Settings")]
    public float arcHeight = 10f;     // How high the rainbow goes
    public float travelTime = 2.3f;   // Always takes exactly 1.5 seconds to land
    public float explosionRadius = 4f;

    public GameObject explosionPrefab;

    private float damage;
    private LayerMask targetLayer;

    // The 3 Bezier Points
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 controlPoint;

    private float timeElapsed = 0f;

    // We call this right after Instantiate()
    public void Launch(Vector3 start, Vector3 end, float dmg, LayerMask layer)
    {
        damage = dmg;
        targetLayer = layer;
        startPos = start;
        targetPos = end;

        // Find the halfway point on the ground
        Vector3 halfway = startPos + (targetPos - startPos) / 2f;

        // Push it up into the sky to create the arc!
        controlPoint = new Vector3(halfway.x, Mathf.Max(startPos.y, targetPos.y) + arcHeight, halfway.z);
    }

    void Update()
    {
        // 1. Progress our timer (t goes from 0.0 to 1.0)
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / travelTime;

        // 2. Are we there yet?
        if (t >= 1f)
        {
            Explode();
            return;
        }

        // 3. The Bezier Curve Math (Unity makes this easy by combining two normal Lerps!)
        Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
        Vector3 m2 = Vector3.Lerp(controlPoint, targetPos, t);
        transform.position = Vector3.Lerp(m1, m2, t);

        // 4. Polish: Make the bullet point in the direction it is falling
        Vector3 nextPos = Vector3.Lerp(m1, m2, t + 0.01f);
        if (nextPos != transform.position)
        {
            transform.rotation = Quaternion.LookRotation(nextPos - transform.position);
        }
    }

    void Explode()
    {
        // AoE Damage Logic (Same as your Fireball!)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer);
        foreach (Collider hit in hitColliders)
        {
            Unit enemy = hit.GetComponentInParent<Unit>();
            if (enemy != null) enemy.TakeDamage(damage);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

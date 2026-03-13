using UnityEngine;

public class DemolisherAI : EnemyAI
{
    public float explosionRadius = 5f;
    public GameObject explosionPrefab;
    public LayerMask targetLayer;

    protected override void PerformAttack()
    {
        Explode();
    }

    public void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer);

        foreach (Collider hit in targets)
        {
            Unit friendly = hit.GetComponent<Unit>();
            Building building = hit.GetComponentInParent<Building>();

            if (friendly != null)
            {
                friendly.TakeDamage(stats.damage);
            }
            if(building != null)
            {
                building.TakeDamage(stats.damage);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

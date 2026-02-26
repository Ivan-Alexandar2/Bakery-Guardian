using UnityEngine;

public class FireBall : Projectile
{
    private float sphereRadius = 3f;
    [SerializeField] private GameObject explosionPrefab;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 1. Find everything inside the blast radius that matches the targetLayer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius, targetLayer);

        // 2. Loop through the victims and apply damage
        foreach (Collider hit in hitColliders)
        {
            // Look for the Unit script (check parent just in case the collider is on a child object)
            Unit enemy = hit.GetComponentInParent<Unit>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

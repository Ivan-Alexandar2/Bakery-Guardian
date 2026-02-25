using UnityEngine;

public class FireBall : Projectile
{
    private float sphereRadius = 3f;

    protected override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);


        Explode();
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

        // Optional: Spawn a cool explosion particle effect here!
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 3. Destroy the Fireball
        Destroy(gameObject);
    }
}

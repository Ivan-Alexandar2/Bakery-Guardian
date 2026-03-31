using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Molotov : Projectile
{
    public GameObject firePrefab;
    //public float speed = 15f;
    //private LayerMask targetLayer;

    // physics and stuff
    public float throwForce = 10f;
    public float upwardArc = 2f;
    private bool hasDeployed = false;

    public void Setup(LayerMask layer, Collider shooterCollider)
    {
        targetLayer = layer;

        if (shooterCollider != null)
        {
            Collider myCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(myCollider, shooterCollider);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Ensure physics is ON
        Vector3 force = transform.forward * throwForce + Vector3.up * upwardArc;
        rb.AddForce(force, ForceMode.Impulse);

        Destroy(gameObject, 5f); // Safety cleanup
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) // Collision and not trigger for testing the molly dropping only on the ground
    {                                                  // and not trigger on units
        if (hasDeployed) return;
        if(collision.transform.CompareTag("Ground"))
        {
            hasDeployed = true;
            DeployFireArea();
            Destroy(gameObject);
        }     
    }

    void DeployFireArea()
    {
        // Spawn slightly up so it doesn't clip into the ground
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject area = Instantiate(firePrefab, spawnPos, Quaternion.identity);

        area.GetComponent<FireArea>().Setup(targetLayer);
    }    
}

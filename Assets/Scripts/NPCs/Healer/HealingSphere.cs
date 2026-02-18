using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // shot sum prost
public class HealingSphere : MonoBehaviour
{
    public GameObject healingAreaPrefab;
    public float speed = 15f;
    private LayerMask targetLayer;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (hasDeployed) return;

        hasDeployed = true;
        DeployHealingArea();
        Destroy(gameObject);
    }

    void DeployHealingArea()
    {
        // Spawn slightly up so it doesn't clip into the ground
        Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
        GameObject area = Instantiate(healingAreaPrefab, spawnPos, Quaternion.identity);

        // Pass the target layer to the area
        area.GetComponent<HealingArea>().Setup(targetLayer);
    }
}

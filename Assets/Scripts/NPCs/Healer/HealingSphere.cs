using UnityEngine;

public class HealingSphere : MonoBehaviour
{
    public GameObject healingAreaPrefab;
    public float speed = 15f;
    private LayerMask allyLayer;

    public void Setup(LayerMask layer)
    {
        allyLayer = layer;
        Destroy(gameObject, 5f); // Safety cleanup
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit the ground OR an ally?
        // Simple check: Just explode on anything except the shooter?
        // Ideally: Explode if we hit the floor or the target.
        GameObject areaObj = Instantiate(healingAreaPrefab, transform.position, Quaternion.identity);
        HealingArea areaScript = areaObj.GetComponent<HealingArea>();
        areaScript.Setup(allyLayer);
        Destroy(gameObject);
    }
}

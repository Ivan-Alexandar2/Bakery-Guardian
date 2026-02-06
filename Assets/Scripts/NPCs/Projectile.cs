using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    public LayerMask targetLayer;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Auto-cleanup
    }

    private void Update()
    {
        // Move Forward relative to rotation
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check Layer (Are we hitting a valid target?)
        // Bitwise math check: Is the object's layer inside our targetLayer mask?
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 2. Deal Damage (Try Unit, then Building)
            if (other.TryGetComponent(out Unit unit))
            {
                unit.TakeDamage(damage);
            }
            else if (other.TryGetComponent(out Building building))
            {
                building.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    // Setup method for the Shooter to call
    public void Setup(float newDamage, LayerMask newLayer)
    {
        damage = newDamage;
        targetLayer = newLayer;
    }
}

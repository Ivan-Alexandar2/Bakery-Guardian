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

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 1. Check Layer (Are we hitting a valid target?)
        // Bitwise math check: Is the object's layer inside our targetLayer mask?
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            bool hitSomething = false;

            Unit unit = other.GetComponentInParent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
                hitSomething = true;
            }

            else
            {
                Building building = other.GetComponentInParent<Building>();
                if (building != null)
                {
                    building.TakeDamage(damage);
                    hitSomething = true;
                }
            }

            if (hitSomething)
            {
                Destroy(gameObject);
            }
        }
    }

    // Setup method for the Shooter to call
    public void Setup(float newDamage, LayerMask newLayer, float newSpeed)
    {
        damage = newDamage;
        targetLayer = newLayer;
        speed = newSpeed;
    }
}

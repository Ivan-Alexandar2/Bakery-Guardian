using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FireArea : MonoBehaviour
{
    public float damageAmount = 3f;
    public float duration = 4f;
    public float burnRate = 0.3f;
    public LayerMask targetLayer;

    private List<Unit> burningUnits = new List<Unit>();
    private List<Building> burningBuildings = new();
    private float timer;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        // Heal Tick: Run this logic every 1 second (or every frame if you want fast healing)
        timer += Time.deltaTime;
        if (timer >= burnRate)
        {
            BurnEveryone();
            timer = 0;
        }
    }

    void BurnEveryone()
    {
        // Loop backwards when removing items or checking nulls!
        for (int i = burningUnits.Count - 1; i >= 0; i--)
        {
            Unit u = burningUnits[i];

            // 1. Check if they died or disappeared
            if (u == null)
            {
                burningUnits.RemoveAt(i);
                continue;
            }

            // 2. Burn them
            u.TakeDamage(damageAmount);
        }

        // Loop for any buildings
        for (int i = burningBuildings.Count - 1; i >= 0; i--)
        {
            Building b = burningBuildings[i];

            // 1. Check if they died or disappeared
            if (b == null)
            {
                burningBuildings.RemoveAt(i);
                continue;
            }

            // 2. Burn them
            b.TakeDamage(damageAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check Layer
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 2. Find Unit
            Unit unit = other.GetComponentInParent<Unit>();
            Building building = other.GetComponentInParent<Building>(); // or getcomponent

            if (unit != null && !burningUnits.Contains(unit))
            {
                burningUnits.Add(unit);
                Debug.Log($"<color=orange>Fire Area: Added {unit.name}</color>");
            }
            if (building != null && !burningBuildings.Contains(building))
            {
                burningBuildings.Add(building);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Check Layer again to avoid unnecessary GetComponent calls
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Unit unit = other.GetComponentInParent<Unit>();
            Building building = other.GetComponentInParent<Building>();

            // 2. Remove from List
            if (unit != null && burningUnits.Contains(unit))
            {
                burningUnits.Remove(unit);
            }
            if(building != null && burningBuildings.Contains(building)) 
            {
                burningBuildings.Remove(building);
            }
        }
    }

    // Call this from the Projectile script before it spawns the area!
    public void Setup(LayerMask layerToHeal)
    {
        targetLayer = layerToHeal;
        Debug.Log($"Healing Area looking for layer: {targetLayer.value}");
    }
}

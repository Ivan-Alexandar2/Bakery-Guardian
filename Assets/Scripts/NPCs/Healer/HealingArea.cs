using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealingArea : MonoBehaviour
{
    public float healAmount = 5f;
    public float duration = 3f;
    public float healRate = 1f;
    public LayerMask targetLayer;

    private List<Unit> patients = new List<Unit>();
    private float timer;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        // Heal Tick: Run this logic every 1 second (or every frame if you want fast healing)
        timer += Time.deltaTime;
        if (timer >= healRate)
        {
            HealEveryone();
            timer = 0;
        }
    }

    void HealEveryone()
    {
        // Loop backwards when removing items or checking nulls!
        for (int i = patients.Count - 1; i >= 0; i--)
        {
            Unit u = patients[i];

            // 1. Check if they died or disappeared
            if (u == null)
            {
                patients.RemoveAt(i);
                continue;
            }

            // 2. Heal them
            u.Heal(healAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check Layer (Is it an ally?)
        // If yes, Get Component<Unit>
        // If not null AND not already in list -> patients.Add(unit);

        // 1. Check Layer
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 2. Find Unit
            Unit unit = other.GetComponentInParent<Unit>();

            if (unit != null && !patients.Contains(unit))
            {
                patients.Add(unit);
                Debug.Log($"<color=green>Healing Area: Added {unit.name}</color>");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Check Layer again to avoid unnecessary GetComponent calls
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Unit unit = other.GetComponentInParent<Unit>();

            // 2. Remove from List
            if (unit != null && patients.Contains(unit))
            {
                patients.Remove(unit);
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

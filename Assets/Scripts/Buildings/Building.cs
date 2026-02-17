using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class Building : MonoBehaviour
{
    public List<ResourceCost> buildingCost = new List<ResourceCost>();
    public List<ResourceCost> unitSpawnCost = new List<ResourceCost>();

    public float health;
    public float maxHealth;

    [Header("Economy")]
    public ResourceType costType;
    public int buildCost = 100;

    public float timeForNPCSpawn;
    public Sprite icon;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Logic: If this is the main base, Game Over?
        if (gameObject.CompareTag("MainBase"))
        {
            Debug.Log("GAME OVER");
            //Time.timeScale = 0; // Pause game
        }

        // 2. Logic: Eject workers (The "Refugee" logic we discussed)
        // (We will add this later)

        // 3. Destroy
        Destroy(gameObject);
    }

    public void Demolish()
    {
        // 1. Calculate Refund (50%)
        int refundAmount = Mathf.FloorToInt(buildCost * 0.5f);
        GameManager manager = FindObjectOfType<GameManager>();
        // 2. Give back resources
        // (Ensure GameManager is accessible!)
        manager.AddResource(costType, refundAmount);

        Debug.Log($"Demolished {name}. Refunded {refundAmount} {costType}.");

        // 3. Die (This handles the explosion/destroy logic you already have)
        Die();
    }
}

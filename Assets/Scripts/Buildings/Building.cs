using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public List<ResourceCost> buildingCost = new List<ResourceCost>();
    public List<ResourceCost> unitSpawnCost = new List<ResourceCost>();

    public float health;
    public float maxHealth;

    [Header("Economy")]
    public ResourceType costType;
    public int buildCost = 100;

    [Header("Worker Management")]
    public int maxWorkers = 3;
    public List<WorkerAI> currentWorkers = new List<WorkerAI>();

    [Header("Tooltip Info")]
    public string buildingDisplayName; // e.g., "Barracks"
    public string spawnDescription;    // e.g., "Spawns: Melee Soldiers"

    public float timeForNPCSpawn;
    public Sprite icon;
    public string jobType; // Type "Bakery", "Hospital"...
    void Start()
    {
        health = maxHealth;
        GameManager.Instance.RegisterWorkplace(this);
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
        foreach (WorkerAI worker in currentWorkers)
        {
            if (worker != null)
            {
                // Call our new dedicated wake-up method!
                worker.EvictFromBuilding();
            }
        }

        currentWorkers.Clear();
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

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterWorkplace(this);
    }

    public bool HasFreeSpace()
    {
        UnitSpawner spawner = GetComponent<UnitSpawner>();
        if (spawner != null)
        {
            return !spawner.IsFull; // Uses your existing limit!
        }
        return false;
    }

    public void AdoptWorker(WorkerAI worker)
    {
        UnitSpawner spawner = GetComponent<UnitSpawner>();
        if (spawner != null)
        {
            spawner.AdoptUnit(worker.gameObject);
        }

        AddWorker(worker);
    }

    public void AddWorker(WorkerAI worker)
    {
        if (!currentWorkers.Contains(worker))
        {
            currentWorkers.Add(worker);
        }
    }

    public void RemoveWorker(WorkerAI worker)
    {
        if (currentWorkers.Contains(worker))
        {
            currentWorkers.Remove(worker);
        }
    }
}

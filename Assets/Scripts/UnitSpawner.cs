using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab;
    public Transform spawnPoint;

    // We get these settings from our OWN neighbor script
    private Building myBuildingStats;

    private int queuedUnits;
    private float currentTimer;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Get the stats from the script attached to THIS SAME OBJECT
        myBuildingStats = GetComponent<Building>();
    }

    void Update()
    {
        // FACTORY LOGIC
        if (queuedUnits > 0)
        {
            currentTimer += Time.deltaTime;

            // Use 'myBuildingStats' instead of asking SelectionManager
            if (currentTimer >= myBuildingStats.timeForNPCSpawn)
            {
                SpawnUnit();
                currentTimer = 0;
                queuedUnits--;
            }
        }
    }

    private void SpawnUnit()
    {
        Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Unit Spawned!");
    }

    // Called by the UI Manager
    public void AttemptQueueUnit()
    {
        // Use the cost from our own building script
        List<ResourceCost> cost = myBuildingStats.unitSpawnCost;

        if (gameManager.TryBuyUnit(cost))
        {
            queuedUnits++;
            Debug.Log("Unit Queued. Total: " + queuedUnits);
        }
        else
        {
            Debug.Log("Not enough minerals.");
        }
    }

    // Helper for visualization later
    public float GetProgress()
    {
        if (queuedUnits == 0) return 0;
        return currentTimer / myBuildingStats.timeForNPCSpawn;
    }

    public float GetTimeRemaining()
    {
        if (queuedUnits == 0) return 0;
        return myBuildingStats.timeForNPCSpawn - currentTimer;
    }

    public int GetQueueCount() => queuedUnits;
}

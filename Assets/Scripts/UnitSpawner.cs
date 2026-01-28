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
        GameObject newUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

        // If the new unit is a Worker, tell it "I am your workplace"
        WorkerAI workerScript = newUnit.GetComponent<WorkerAI>();
        if (workerScript != null)
        {
            workerScript.myWorkplace = this.GetComponent<Building>();
        }
    }

    // Called by the UI Manager
    public void AttemptQueueUnit()
    {
        // Use the cost from our own building script
        List<ResourceCost> cost = myBuildingStats.unitSpawnCost;

        if (gameManager.TryBuyUnit(cost))
        {
            queuedUnits++;
        }
        else
        {
            Debug.Log("Not enough materials.");
        }
    }

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

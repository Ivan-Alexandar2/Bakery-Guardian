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
    public int maxUnitCount = 5;

    public bool IsFull
    {
        get
        {
            aliveUnits.RemoveAll(u => u == null);
            return (aliveUnits.Count + queuedUnits) >= maxUnitCount;
        }
    }

    [SerializeField] private List<GameObject> aliveUnits = new List<GameObject>();

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
        // 1. Clean the list (Remove dead units)
        // This is a quick way to ensure the count is accurate before we check it
        aliveUnits.RemoveAll(item => item == null);

        if (aliveUnits.Count >= maxUnitCount)
        {
            Debug.Log("Unit Limit Reached!");
            return; // Stop here
        }

        GameObject newUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

        // If the new unit is a Worker, tell it "I am your workplace"
        WorkerAI workerScript = newUnit.GetComponent<WorkerAI>();
        if (workerScript != null)
        {
            workerScript.myWorkplace = GetComponent<Building>();
        }

        aliveUnits.Add(newUnit);
    }

    // Called by the UI Manager
    public bool AttemptQueueUnit()
    {
        // Use the cost from our own building script
        List<ResourceCost> cost = myBuildingStats.unitSpawnCost;

        // THE ONLY PAYMENT CHECK
        if (gameManager.TryBuyUnit(cost))
        {
            queuedUnits++;
            return true; // Purchase Successful
        }
        else
        {
            return false; // Purchase Failed
        }
    }

    public void AdoptUnit(GameObject refugee)
    {
        if (!aliveUnits.Contains(refugee))
        {
            aliveUnits.Add(refugee);
            Debug.Log($"{gameObject.name} adopted a refugee! Total workers: {aliveUnits.Count}");
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

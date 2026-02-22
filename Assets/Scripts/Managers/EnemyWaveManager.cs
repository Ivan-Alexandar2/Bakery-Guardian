using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The manager that handles the enemy spawning system
/// </summary>
public class EnemyWaveManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<UnitStats> enemyCatalog; // Drag ALL enemy scriptable objects here
    public Transform mainBase;
    public float mapRadius = 50f;
    public float safeZoneRadius = 15f;   // Don't spawn closer than this to base

    [Header("Difficulty Settings")]
    public int baseBudget = 10;
    public int pointsPerDay = 5;         // +5 points every night

    private void Start()
    {
        // Listen for the Night Bell
        DayNightManager.Instance.OnNightStart += StartWave;

        if (mainBase == null)
            mainBase = GameObject.FindGameObjectWithTag("MainBase").transform;
    }

    private void OnDestroy()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnNightStart -= StartWave;
    }

    void StartWave()
    {
        int day = DayNightManager.Instance.currentDay;
        int budget = baseBudget + (day * pointsPerDay);

        Debug.Log($"Night {day} Started! Budget: {budget}");

        // 1. Go Shopping
        List<UnitStats> waveQueue = GenerateWave(budget, day);

        // 2. Start the Invasion (Spread over 60 seconds)
        StartCoroutine(SpawnRoutine(waveQueue));
    }

    List<UnitStats> GenerateWave(int budget, int currentDay) // most complicated
    {
        List<UnitStats> queue = new List<UnitStats>();
        List<UnitStats> availableEnemies = new List<UnitStats>();

        // Filter: What enemies are unlocked yet?
        foreach (var enemy in enemyCatalog)
        {
            if (currentDay >= enemy.minDayToSpawn)
            {
                availableEnemies.Add(enemy);
            }
        }

        // Safety: If no enemies are valid, return empty
        if (availableEnemies.Count == 0) return queue;

        // Buying Loop
        int attempts = 0; // Safety break to prevent infinite loops
        while (budget > 0 && attempts < 100)
        {
            // Pick a random unlocked enemy
            UnitStats pick = availableEnemies[Random.Range(0, availableEnemies.Count)];

            if (budget >= pick.spawnCost)
            {
                queue.Add(pick);
                budget -= pick.spawnCost;
            }
            else
            {
                // Can't afford this specific one. 
                // In a complex system, we would filter for cheaper ones here.
                // For now, we just try again.
                attempts++;
            }
        }

        return queue;
    }

    IEnumerator SpawnRoutine(List<UnitStats> enemiesToSpawn)
    {
        foreach (UnitStats stats in enemiesToSpawn)
        {
            // Wait a random bit so they don't all appear at once
            float waitTime = Random.Range(0.5f, 5f);
            yield return new WaitForSeconds(waitTime);

            SpawnEnemy(stats);
        }
    }

    void SpawnEnemy(UnitStats stats)
    {
        Vector3 spawnPos = GetValidSpawnPosition();

        if (spawnPos != Vector3.zero)
        {
            GameObject newEnemy = Instantiate(stats.enemyPrefab, spawnPos, Quaternion.identity);

            // Setup logic (if needed)
            // Example: EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 10; i++) // Try 10 times to find a valid spot
        {
            // 1. Pick random point in circle
            Vector2 randomCircle = Random.insideUnitCircle * mapRadius;
            Vector3 potentialPos = new Vector3(randomCircle.x, 0, randomCircle.y) + mainBase.position;

            // 2. Check Distance (Too close to base?)
            if (Vector3.Distance(potentialPos, mainBase.position) < safeZoneRadius)
                continue;

            // 3. Check NavMesh (Is it on a mountain/water?)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position; // Found a valid spot on the blue mesh!
            }
        }

        Debug.LogWarning("Could not find spawn point!");
        return Vector3.zero;
    }
}
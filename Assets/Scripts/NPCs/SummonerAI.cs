using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonerAI : RangedAI
{
    // How it works: A standard ranged unit that shoots fireballs, but also spawns minions.
    // Minions will have a different AI script
    // The summoner spawns a minion every 15 seconds ONLY IF they haven't exceeded the max amount of minions alive

    [Header("Summoner vars")]
    public List<SoldierAI> aliveMinions = new();
    public SoldierAI minionPrefab;
    public float timeBeforeSpawn = 5f;
    public float maxMinions;

    private bool isSpawning;

    void Start()
    {
        base.Start();
    }

    private void Update()
    {
        base.Update();

        if (isSpawning) return; // optimization 

        aliveMinions.RemoveAll(u => u == null);
        if (aliveMinions.Count < maxMinions && !isSpawning)
        {
            StartCoroutine(SummonCoroutine());
        }
    }
    public IEnumerator SummonCoroutine()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBeforeSpawn);
        SoldierAI minion = Instantiate(minionPrefab, firePoint.position, transform.rotation);
        minion.guardPoint = gameObject.transform;
        aliveMinions.Add(minion);
        isSpawning = false;
        
    }
}

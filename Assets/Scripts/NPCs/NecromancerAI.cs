using System.Collections;
using UnityEngine;

public class NecromancerAI : EnemyAI
{
    [Header("Summoner vars")]
    public SoldierAI[] minionPrefabs;
    public float timeBeforeSpawn = 7f;

    private bool isSpawning;

    void Start()
    {
        base.Start();
    }

    private void Update()
    {
        base.Update();

        if (isSpawning) return; // optimization 

        isSpawning = true;

        StartCoroutine(SummonCoroutine());
    }
    public IEnumerator SummonCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeSpawn);

        int randomIndex = Random.Range(0, minionPrefabs.Length);
        SoldierAI selectedMinion = minionPrefabs[randomIndex];

        SoldierAI minion = Instantiate(selectedMinion, transform.position, transform.rotation);
        minion.guardPoint = gameObject.transform;

        isSpawning = false;
    }
}

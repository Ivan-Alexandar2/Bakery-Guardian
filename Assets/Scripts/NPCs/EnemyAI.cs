using UnityEngine;

public class EnemyAI : SoldierAI
{
    protected override void Start()
    {
        GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
        if (baseObj != null)
        {
            guardPoint = baseObj.transform;
        }

        patrolRadius = 5f; // Keep a tight circle around the base once they arrive
        patrolWaitTime = 0.5f; // Don't wait long, keep moving

        // 3. Run the standard Soldier setup
        base.Start();
    }
}

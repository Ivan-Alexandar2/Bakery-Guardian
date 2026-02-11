using UnityEngine;

public class EnemyRangedAI : RangedAI // Inherits Shooting & Soldier logic
{
    protected override void Start()
    {
        // 1. Call the base start so Health Bar spawns!
        base.Start();

        // 2. Set the Siege Target (Main Base)
        GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
        if (baseObj != null)
        {
            guardPoint = baseObj.transform;
        }

        // 3. Tweak Patrol settings to be aggressive
        patrolRadius = 5f;
        patrolWaitTime = 0.5f;
    }

    // We don't need to override Update because RangedAI's update (inherited from SoldierAI) 
    // already handles "Chase -> Attack". 
    // By setting 'guardPoint' to the Base, the "Patrol" state becomes a "March" state.
}
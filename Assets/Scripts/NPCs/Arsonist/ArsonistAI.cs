using UnityEngine;

public class ArsonistAI : EnemyRangedAI
{
    public float journeyTime = 1.0f;
    private float throwTime;
    
    void Start()
    {
        base.Start();

        throwTime = Time.time;
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();

        Vector3 center = (firePoint.transform.position - currentTarget.position) * 0.5f;

        center -= new Vector3(0, 1, 0);

        Vector3 selfRelCenter = firePoint.transform.position - center;
        Vector3 targetRelCenter = currentTarget.position - center;

        float fracComplete = (Time.time - throwTime) / journeyTime;

        projectilePrefab.transform.position = Vector3.Slerp(selfRelCenter, targetRelCenter, fracComplete);
        projectilePrefab.transform.position += center;
    }
}

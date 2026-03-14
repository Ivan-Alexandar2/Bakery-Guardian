using UnityEngine;

public class PlaguebearerAI : EnemyAI
{
    [Header("Plague Settings")]
    public float coneAngle = 75f;       // How wide the cone is (90 degrees total)
    public float infectionDamage = 2f;
    public ParticleSystem vomitParticles;

    protected override void PerformAttack()
    {
        attackCooldown = stats.attackSpeed;

        // 1. Play the visual effect
        if (vomitParticles != null) vomitParticles.Play();

        // 2. Find everyone in range
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, stats.attackRange + 2, sensor.targetLayer);

        foreach (Collider col in targetsInRange)
        {
            // 3. Mathematical Cone Check
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            dirToTarget.y = 0; // Keep it flat

            float angle = Vector3.Angle(transform.forward, dirToTarget);

            // If they are inside the cone
            if (angle <= coneAngle)
            {
                Unit victim = col.GetComponentInParent<Unit>();
                if (victim != null)
                {
                    victim.Infect(infectionDamage);
                    victim.TakeDamage(stats.damage); 
                }
            }
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.AI;

// Require component ensures we never forget the NavMeshAgent
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Setup")]
    public UnitStats stats; // Drag the ScriptableObject here

    // Runtime variables
    protected float currentHealth;
    protected NavMeshAgent agent;

    // Events for other scripts to listen to (like Health Bar UI)
    public Action<float, float> OnHealthChanged; // Current, Max
    public Action OnDeath;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        // Initialize from the ScriptableObject
        if (stats != null)
        {
            currentHealth = stats.maxHealth;
            agent.speed = stats.moveSpeed;
        }
        else
        {
            Debug.LogError("Unit Stats missing on " + gameObject.name);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}

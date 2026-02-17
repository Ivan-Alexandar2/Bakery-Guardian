using System;
using UnityEngine;
using UnityEngine.AI;

// Require component ensures we never forget the NavMeshAgent
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Setup")]
    public UnitStats stats;

    // Runtime variables
    internal protected float currentHealth; // used in HealerSensor
    protected NavMeshAgent agent;

    // Events for other scripts to listen to (like Health Bar UI)
    public Action<float, float> OnHealthChanged; // Current, Max
    public Action OnDeath;

    [Header("UI")]
    public HealthBar healthBarPrefab; // Drag the Prefab here
    [SerializeField] private HealthBar myHealthBar;

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

        if (healthBarPrefab != null)
        {
            myHealthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);

            // Determine Color
            Color teamColor = Color.green; // Default Ally
            if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                teamColor = Color.red;
            }

            myHealthBar.Setup(transform, stats.maxHealth, teamColor);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);
        myHealthBar.UpdateHealth(currentHealth, stats.maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > stats.maxHealth) currentHealth = stats.maxHealth;

        if (myHealthBar != null) myHealthBar.UpdateHealth(currentHealth, stats.maxHealth);
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();

        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Roll the dice (0 to 100)
            float roll = UnityEngine.Random.Range(0f, 100f);
            GameManager manager = FindObjectOfType<GameManager>();


            if (roll <= stats.gemDropChance)
            {
                // Winner!
                Debug.Log($"<color=cyan>GEM DROP! Rolled {roll} vs {stats.gemDropChance}</color>");

                // Add to Global Inventory
                // (Assuming GameManager is Singleton "Instance")
                manager.AddResource(ResourceType.Gems, stats.gemAmount);

                // Optional: Spawn a visual "Floating Gem" effect here later!
            }
        }

        Destroy(gameObject);
    }

    // Health bar cleanup stuff
    void OnDisable()
    {
        if (myHealthBar != null) myHealthBar.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (myHealthBar != null) myHealthBar.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (myHealthBar != null) Destroy(myHealthBar.gameObject);
    }
}

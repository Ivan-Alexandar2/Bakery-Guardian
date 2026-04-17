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
    public HealthBar healthBarPrefab;
    [SerializeField] private HealthBar myHealthBar;

    [Header("Status Effects")]
    public bool isPlagued = false;
    private float plagueDamage = 2f;
    private float plagueTickRate = 1f;
    private float plagueTimer = 0f;

    // Visuals
    private Renderer[] myRenderers;     // Array in case the model has multiple parts
    private Color[] originalColors;     // To remember what color they used to be

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

        // Grab all renderers in this object and its children
        myRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[myRenderers.Length];

        // Save their default colors so we can revert back later
        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (myRenderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = myRenderers[i].material.color;
            }
        }
    }

    protected virtual void Update()
    {
        if (isPlagued)
        {
            plagueTimer -= Time.deltaTime;
            if (plagueTimer <= 0)
            {
                TakeDamage(plagueDamage);
                plagueTimer = plagueTickRate;
            }
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

    public void Infect(float damageAmount)
    {
        if (isPlagued) return; // Don't infect twice

        isPlagued = true;
        plagueDamage = damageAmount;
        plagueTimer = plagueTickRate; // Trigger first tick immediately

        // Turn them sickly green
        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (myRenderers[i].material.HasProperty("_Color"))
            {
                // Blend their original color with bright green
                myRenderers[i].material.color = Color.Lerp(originalColors[i], Color.green, 0.5f);
            }
        }
    }

    public void CurePlague()
    {
        if (!isPlagued) return;

        isPlagued = false;

        // Revert to original color
        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (myRenderers[i].material.HasProperty("_Color"))
            {
                myRenderers[i].material.color = originalColors[i];
            }
        }
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();

        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Roll the dice (0 to 100)
            float roll = UnityEngine.Random.Range(0f, 100f);

            if (roll <= stats.gemDropChance)
            {
                //Debug.Log($"<color=cyan>GEM DROP! Rolled {roll} vs {stats.gemDropChance}</color>");

                GameManager.Instance.AddResource(ResourceType.Gems, stats.gemAmount);
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

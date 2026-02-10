using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitStats", menuName = "Unit/New Unit Stats")]
public class UnitStats : ScriptableObject
{
    [Header("General Info")]
    public string unitName;
    public UnitType unitType; // Enum defined below

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackSpeed = 1.5f;

    [Header("Economy (Workers Only)")]
    public int resourceCapacity = 1;
    public float harvestTime = 8.0f;

    [Header("Wave Spawning (Enemies Only)")]
    public int spawnCost = 5;      // How many points this enemy costs
    public int minDayToSpawn = 1;  // Unlocks on Day X
    public GameObject enemyPrefab; // The actual thing to spawn

    [Header("Loot")]
    [Range(0, 100)]
    public float gemDropChance = 20f; // Default 20%
    public int gemAmount = 1;
}

public enum UnitType
{
    Worker,
    Soldier,
    Enemy
}

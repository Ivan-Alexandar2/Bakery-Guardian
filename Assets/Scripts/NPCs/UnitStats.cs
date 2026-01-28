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
}

public enum UnitType
{
    Worker,
    Soldier,
    Enemy
}

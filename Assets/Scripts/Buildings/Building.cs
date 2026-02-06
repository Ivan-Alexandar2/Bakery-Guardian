using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public List<ResourceCost> buildingCost = new List<ResourceCost>();
    public List<ResourceCost> unitSpawnCost = new List<ResourceCost>();

    public float health;
    public float maxHealth;

    //public GameObject NPC; // will be an NPC script in the future
    public float timeForNPCSpawn;
    public Sprite icon;

    void Start()
    {
        // Only if I forget to set current health in inspector
        if (health == 0) health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Logic: If this is the main base, Game Over?
        if (gameObject.CompareTag("MainBase"))
        {
            Debug.Log("GAME OVER");
            //Time.timeScale = 0; // Pause game
        }

        // 2. Logic: Eject workers (The "Refugee" logic we discussed)
        // (We will add this later)

        // 3. Destroy
        Destroy(gameObject);
    }
}

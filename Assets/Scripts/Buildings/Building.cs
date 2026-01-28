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
}

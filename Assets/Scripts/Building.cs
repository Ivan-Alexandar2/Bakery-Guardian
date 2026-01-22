using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public List<ResourceCost> buildingCost = new List<ResourceCost>();
    public List<ResourceCost> npcSpawnCost = new List<ResourceCost>();

    public float health;
    public GameObject NPC; // will be an NPC script in the future
    public Sprite icon;

}

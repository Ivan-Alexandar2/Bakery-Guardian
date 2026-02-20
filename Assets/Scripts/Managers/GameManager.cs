using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct ResourceCost
{
    public ResourceType type;
    public int amount;
}

public enum ResourceType
{
    Wood,
    Bread,
    Stone,
    Fish,
    Gems
}
public class GameManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> resourceInventory = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, TextMeshProUGUI> uiReferences = new Dictionary<ResourceType, TextMeshProUGUI>();

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI breadText;
    [SerializeField] private TextMeshProUGUI fishText;
    [SerializeField] private TextMeshProUGUI gemsText;

    public static GameManager Instance;

    public List<Building> allWorkplaces = new List<Building>();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
       resourceInventory.Add(ResourceType.Wood, 80);
       resourceInventory.Add(ResourceType.Bread, 60);
       resourceInventory.Add(ResourceType.Fish, 0);
       resourceInventory.Add(ResourceType.Stone, 0);
       resourceInventory.Add(ResourceType.Gems, 0);

       uiReferences.Add(ResourceType.Wood, woodText);
       uiReferences.Add(ResourceType.Bread, breadText);
       uiReferences.Add(ResourceType.Fish, fishText);
       uiReferences.Add(ResourceType.Stone, stoneText);
       uiReferences.Add(ResourceType.Gems, gemsText);

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Time.timeScale = 0f;
        if (Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) Time.timeScale = 3f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) Time.timeScale = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) Time.timeScale = 5f;

    }

    private void UpdateUI() // The method that actually updates the text
    {
        foreach (var resource in resourceInventory)
        {
            // resource.Key is the Type (Wood)
            // resource.Value is the Amount (100)

            // Find the matching text box and update it
            if (uiReferences.ContainsKey(resource.Key))
            {
                uiReferences[resource.Key].text = resource.Value.ToString();
            }
        }
    }   

    public bool TryBuyBuilding(List<ResourceCost> buildingCosts)
    {
        foreach(ResourceCost cost in buildingCosts)
        {
            if (resourceInventory[cost.type] < cost.amount)
            {
                Debug.Log("Not enough resources");
                return false;
            }
        }

        foreach (ResourceCost cost in buildingCosts)
        {
            resourceInventory[cost.type] -= cost.amount;
        }

        UpdateUI();
        return true;
    }

    public bool TryBuyUnit(List<ResourceCost> unitCost)
    {
        foreach (ResourceCost cost in unitCost)
        {
            if (resourceInventory[cost.type] < cost.amount)
            {
                Debug.Log("Not enough resources");
                return false;
            }
        }

        foreach (ResourceCost cost in unitCost)
        {
            resourceInventory[cost.type] -= cost.amount;
        }

        UpdateUI();
        return true;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (resourceInventory.ContainsKey(type))
        {
            resourceInventory[type] += amount; // Add to existing pile
        }
        UpdateUI();
    }

    #region BUILDING SEARCH 
    // Helper for Buildings to register themselves
    public void RegisterWorkplace(Building b)
    {
        if (!allWorkplaces.Contains(b)) allWorkplaces.Add(b);
    }

    public void UnregisterWorkplace(Building b)
    {
        if (allWorkplaces.Contains(b)) allWorkplaces.Remove(b);
    }

    // The "Job Search" method
    public Building GetFirstAvailableWorkplace(ResourceType preferredType)
    {
        foreach (Building b in allWorkplaces)
        {
            // Optional: You can filter by ResourceType if you want lumberjacks to stay lumberjacks
            // For now, let's just find ANY open job
            if (b.HasFreeSpace())
            {
                return b;
            }
        }
        return null;
    }
    #endregion
}

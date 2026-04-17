using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The manager that handles resources and any resource related action.
/// It also keeps track of all built structures & lose condition
/// </summary>

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
    [SerializeField] private GameObject losePanel;

    public static GameManager Instance;

    public List<Building> allWorkplaces = new List<Building>();
    [SerializeField] private GameObject mainBakery;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
       resourceInventory.Add(ResourceType.Wood, 10);
       resourceInventory.Add(ResourceType.Bread, 20);
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
        if (Input.GetKeyDown(KeyCode.Alpha0)) Time.timeScale = 0.4f;
        if (Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) Time.timeScale = 3f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) Time.timeScale = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) Time.timeScale = 5f;

        if(mainBakery == null) Lose();

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

    public bool TryBuyUpgrade(List<ResourceCost> upgradeCost)
    {
        foreach (ResourceCost cost in upgradeCost)
        {
            if (resourceInventory[cost.type] < cost.amount)
            {
                Debug.Log("Not enough gems");
                return false;
            }
        }

        foreach (ResourceCost cost in upgradeCost)
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
    public Building GetFirstAvailableWorkplace(string requiredJobType)
    {
        foreach (Building b in allWorkplaces)
        {
            // 1. Must match the exact job type (Bakery == Bakery)
            // 2. Must have free space
            if (b.jobType == requiredJobType && b.HasFreeSpace())
            {
                return b;
            }
        }
        return null; // Nobody is hiring for this job right now
    }
    #endregion

    public void Lose()
    {
        losePanel.SetActive(true);
    }
}

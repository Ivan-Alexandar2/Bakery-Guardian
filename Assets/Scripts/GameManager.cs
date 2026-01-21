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


    void Start()
    {
       resourceInventory.Add(ResourceType.Wood, 5);
       resourceInventory.Add(ResourceType.Bread, 20);

       uiReferences.Add(ResourceType.Wood, woodText);
       uiReferences.Add(ResourceType.Bread, breadText);
       uiReferences.Add(ResourceType.Fish, fishText);
       uiReferences.Add(ResourceType.Stone, stoneText);
       uiReferences.Add(ResourceType.Gems, gemsText);

        UpdateUI();
    }

    void Update()
    {
       
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
}

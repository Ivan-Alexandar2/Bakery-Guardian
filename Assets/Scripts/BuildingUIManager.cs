using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI spawnCostText;
    public Button spawnNPCButton;
    public Image buildingSprite;
    public Image[] resourceSprites;

    public TextMeshProUGUI queueText;
    public TextMeshProUGUI spawnTimer;

    private UnitSpawner currentSpawner;

    public void OpenMenu(Building buildingData)
    {
        menuPanel.SetActive(true);

        // "Play the DVD" -> Copy data from the building to the text
        buildingNameText.text = buildingData.name;
        healthText.text = buildingData.health.ToString();


        if (buildingData.icon != null)
        {
            buildingSprite.sprite = buildingData.icon;
            buildingSprite.gameObject.SetActive(true);
        }
        else
        {
            // Hide the empty white square if you forgot to add an icon
            buildingSprite.gameObject.SetActive(false);
        }

        spawnCostText.text = "";
        foreach (ResourceCost cost in buildingData.unitSpawnCost)
        {
            if (cost.amount > 0)
            {
                // Add a new line for each resource
                // Example result: "Wood: 50\nGold: 100"
                spawnCostText.text += cost.type.ToString() + ": " + cost.amount + "\n";
            }
        }

        currentSpawner = buildingData.GetComponent<UnitSpawner>();
        if (currentSpawner != null)
        {
            spawnNPCButton.interactable = true;
        }
        else
        {
            spawnNPCButton.interactable = false;
        }

        // TODO: Update the cost buttons based on buildingData.npcSpawnCost
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    public void OnSpawnButtonClicked() // LINK THIS TO THE BUTTON
    {
        if (currentSpawner != null)
        {
            currentSpawner.AttemptQueueUnit();
        }
    }
}

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

    [Header("Spawning UI")]
    public Slider spawnProgressBar;
    public TextMeshProUGUI queueText;
    public TextMeshProUGUI timerText;

    public Slider healthSlider;        // Drag the new Health Slider here
    public GameObject spawnToolsGroup;

    private Building currentBuilding;  // We need to keep track of the building for HP updates 
    private UnitSpawner currentSpawner;

    void Update()
    {
        // 1. Optimization: Only update if the menu is actually open!
        if (menuPanel.activeSelf == false) return;
        healthSlider.value = currentBuilding.health;

        if (currentSpawner != null)
        { 
            spawnProgressBar.value = currentSpawner.GetProgress();
            queueText.text = "Queue: " + currentSpawner.GetQueueCount();

            // UPDATE THE TIMER TEXT (Formatted to 1 decimal place, e.g. "3.5s")
            if (currentSpawner != null)
            {
                spawnProgressBar.value = currentSpawner.GetProgress();
                queueText.text = "Queue: " + currentSpawner.GetQueueCount();

                float timeRem = currentSpawner.GetTimeRemaining();
                timerText.text = (timeRem > 0) ? timeRem.ToString("F1") + "s" : "Idle";
            }
        }
    }

    public void OpenMenu(Building buildingData)
    {
        menuPanel.SetActive(true);

        currentBuilding = buildingData; // Save it so Update() can read the health
        currentSpawner = buildingData.GetComponent<UnitSpawner>();

        // "Play the DVD" -> Copy data from the building to the text
        buildingNameText.text = buildingData.name;
        healthText.text = buildingData.health.ToString();

        healthSlider.maxValue = buildingData.maxHealth;
        healthSlider.value = buildingData.health;


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
            spawnToolsGroup.SetActive(true);
            spawnNPCButton.interactable = true;
        }
        else
        {
            spawnToolsGroup.SetActive(false);
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

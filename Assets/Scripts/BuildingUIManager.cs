using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI spawnCostText;
    public Button spawnButton;
    public Image buildingSprite;

    [Header("Spawning UI")]
    public Slider spawnProgressBar;
    public TextMeshProUGUI queueText;
    public TextMeshProUGUI timerText;

    public Slider healthSlider;        // Drag the new Health Slider here
    public GameObject spawnToolsGroup;

    [Header("Upgrade UI")]
    public GameObject upgradePanel;

    private Building currentBuilding;  // We need to keep track of the building for HP updates 
    private UnitSpawner currentSpawner;
    private UpgradeBuilding currentUpgradeBuilding;

    void Update()
    {
        // 1. Optimization: Only update if the menu is actually open!
        if (menuPanel.activeSelf == false) return;
        if (currentBuilding == null) menuPanel.SetActive(false);

        // 2. Update Health Slider
        if (currentBuilding != null)
        {
            healthSlider.value = currentBuilding.health;
            healthText.text = currentBuilding.health.ToString();

            // Update Button Interaction (Grey out if full)
            if (currentSpawner != null)
            {
                spawnButton.interactable = !currentSpawner.IsFull;
            }
        }

        // 3. Update Spawner UI (Progress Bar & Queue)
        if (currentSpawner != null)
        {
            spawnProgressBar.value = currentSpawner.GetProgress();

            // Show Queue count
            queueText.text = "Queue: " + currentSpawner.GetQueueCount();

            // Formatted Timer: "3.5s"
            float timeRem = currentSpawner.GetTimeRemaining();
            timerText.text = (timeRem > 0) ? timeRem.ToString("F1") + "s" : "Idle";
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
            spawnButton.interactable = true;
        }
        else
        {
            spawnToolsGroup.SetActive(false);
            spawnButton.interactable = false;
        }

        currentUpgradeBuilding = buildingData.GetComponent<UpgradeBuilding>();

        if(currentUpgradeBuilding != null)
        {
            upgradePanel.SetActive(true);
        }
        else
        {
            upgradePanel.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void OnSpawnButtonClicked() // LINK THIS TO THE BUTTON
    {
        if (currentSpawner == null) return;

        // 1. CHECK LIMIT
        if (currentSpawner.IsFull)
        {
            Debug.Log("Barracks is full!");
            return;
        }
        currentSpawner.AttemptQueueUnit();
        // 2. ASK SPAWNER TO HANDLE IT
        // We don't touch the GameManager here. The Spawner does.
        //if (currentSpawner.AttemptQueueUnit())
        //{
            
        //}
        //else
        //{
        //    Debug.Log("Cannot afford unit!");
        //}
    }
}

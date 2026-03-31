using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance; // Allows buttons to find this instantly

    [Header("UI References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;

    private void Awake()
    {
        // Setup the Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideTooltip(); // Start hidden
    }

    private void Update()
    {
        // Make the tooltip follow the mouse while it's active
        if (tooltipPanel.activeSelf)
        {
            // Offset by a few pixels so the mouse cursor doesn't cover the text!
            transform.position = Input.mousePosition + new Vector3(15f, -15f, 0f);
        }
    }

    public void ShowTooltip(Building buildingData)
    {
        tooltipPanel.SetActive(true);

        // 1. Text Info
        nameText.text = buildingData.buildingDisplayName;
        descriptionText.text = buildingData.spawnDescription;

        // 2. Dynamic Cost Reading
        costText.text = "Cost:\n";

        foreach (ResourceCost cost in buildingData.buildingCost)
        {
            if (cost.amount > 0)
            {
                costText.text += $"- {cost.amount} {cost.type}\n";
            }
        }
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}

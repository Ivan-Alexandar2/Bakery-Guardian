using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("The Data")]
    public GameObject buildingPrefab;
    public GameObject buildingBlueprint;
    public Building buildingData;

    public GameButtons gameButtons;

    
    //public Building buildingPrefab; // Drag the specific building prefab here!

    public void ClickTroopMenu()
    {
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.troopBuildingMenu.SetActive(false);
    }

    public void ClickResourceMenu()
    {
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.resourceBuildingMenu.SetActive(false);
    }

    public void ClickDefenseMenu()
    {
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.defenseBuildingMenu.SetActive(false);
    }

    // Tooltip panel logic

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mouse hovered over the button!
        if (buildingPrefab != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(buildingData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Mouse left the button!
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Force the tooltip to hide the moment we click to build!
        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
    }
}

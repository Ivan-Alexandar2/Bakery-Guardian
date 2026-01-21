using UnityEngine;
using UnityEngine.UI;

public class BuildButtons : MonoBehaviour
{
    public GameObject buildingPrefab;
    public GameObject buildingBlueprint;

    public GameButtons gameButtons;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Building building;


    public void Awake()
    {
        //gameManager = FindFirstObjectByType<GameManager>();
    }
    public void ClickTroopMenu()
    {
        // Don't instantiate here. Pass the data to the boss.
        //building = buildingPrefab.GetComponent<Building>();
        gameManager.TryBuyBuilding(building.buildingCost);
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.troopBuildingMenu.SetActive(false);
    }

    public void ClickResourceMenu()
    {
        gameManager.TryBuyBuilding(building.buildingCost);
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.resourceBuildingMenu.SetActive(false);
    }

    public void ClickDefenseMenu()
    {
        gameManager.TryBuyBuilding(building.buildingCost);
        FindObjectOfType<BuildingPlacementManager>().StartPlacing(buildingPrefab, buildingBlueprint);
        gameButtons.defenseBuildingMenu.SetActive(false);
    }
}

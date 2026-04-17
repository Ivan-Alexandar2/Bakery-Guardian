using UnityEngine;

public class BuildingUpgradeButton : MonoBehaviour
{
    [Header("Upgraded Entities")]
    public Building buildingPrefab;
    public Unit unitPrefab;

    public GameObject upgradeMenu; // the menu in which all the separate upgrades are displayed
    public Upgrade[] upgrades;

    public float testUpgrade;

    public void OpenUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        // First clear all upgrades and THEN recreate them
        foreach (Transform child in upgradeMenu.transform)
        {
            Destroy(child.gameObject);
        }

        // Then spawn the upgrades
        foreach (var upgrade in upgrades)
        {
            upgrade.SaveUpdateData(upgrade);
            Instantiate(upgrade, upgradeMenu.transform);
        }
    }

    public void Upgrade()
    {

    }

    public void ApplyUpgrades()
    {

    }

    public void GetAllUnits()
    {

    }

    public void GetAllBuildings()
    {

    }

    // The idea - I click on the upgrade building, a panel opens with 2 options - Spawners or Towers, clicking on either would open
    // an upgrade panel with the different spawners/towers options (as buttons), clicking on an option would open the upgrade menu getting
    // the data from that button and displaying its possible upgrades (its different for every building, for example a swordsman hut
    // could have upgrades like "+ swordsman damage" or "+ 1 spawn count", but another building could have different upgrades)
    // , clicking on an upgrade will
    // increase the stated value for every building/unit, take gems and apply those upgrades to every newly spawned unit/building. The 
    // value of that upgrade will then increase

    // 1) GetAllBuildings & GetAllUnits methods are needed to apply the upgrades
    // 2) ApplyUpgrade method is needed
    // 3) An array[] of upgrades for each individual building
    // 4) Call ApplyUpgrade every time a unit spawns (maybe?)
    // 5) Get different data (upgrades) for every building button
}

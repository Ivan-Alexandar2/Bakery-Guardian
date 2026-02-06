using UnityEngine;

public class GameButtons : MonoBehaviour
{
    public GameObject resourceBuildingMenu;
    public GameObject troopBuildingMenu;
    public GameObject defenseBuildingMenu;

    public GameObject currentOpenMenu;

    public void OpenResourceBuildingMenu()
    {
        if (currentOpenMenu != null) currentOpenMenu.SetActive(false);
        currentOpenMenu = null;
        resourceBuildingMenu.SetActive(true);
        currentOpenMenu = resourceBuildingMenu;
    }

    public void OpenTroopBuildingMenu()
    {
        if(currentOpenMenu != null) currentOpenMenu.SetActive(false);
        currentOpenMenu = null; 
        troopBuildingMenu.SetActive(true);
        currentOpenMenu = troopBuildingMenu;
    }

    public void OpenDefenseBuildingMenu()
    {
        if (currentOpenMenu != null) currentOpenMenu.SetActive(false);
        currentOpenMenu = null;
        defenseBuildingMenu.SetActive(true);
        currentOpenMenu = defenseBuildingMenu;
    }

    public void Close()
    {
        currentOpenMenu.SetActive(false);
    }
}

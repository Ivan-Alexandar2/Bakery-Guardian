using UnityEngine;

public class GameButtons : MonoBehaviour
{
    public GameObject resourceBuildingMenu;
    public GameObject troopBuildingMenu;
    public GameObject defenseBuildingMenu;

    public void OpenResourceBuildingMenu()
    {
        resourceBuildingMenu.SetActive(true);
    }

    public void OpenTroopBuildingMenu()
    {
        troopBuildingMenu.SetActive(true);
    }

    public void OpenDefenseBuildingMenu()
    {
        defenseBuildingMenu.SetActive(true);
    }
}

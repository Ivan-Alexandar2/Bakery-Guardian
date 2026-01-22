using UnityEngine;
using UnityEngine.UIElements;

public class SelectionManager : MonoBehaviour
{
    public LayerMask buildingLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // We only want to hit things on the "Building" layer (Optimization!)
            if (Physics.Raycast(ray, out hit, buildingLayer)) // You can add a LayerMask here later
            {
                // "Try" to see if the thing we hit has the Building script
                if (hit.collider.TryGetComponent(out Building building))
                {
                    Debug.Log("You clicked on: " + building.name);
                    SelectBuilding(building);
                }
                else
                {
                    Debug.Log("Didn't click on anything");
                    Deselect();
                }
            }
        }
    }

    void SelectBuilding(Building building)
    {
        FindObjectOfType<BuildingUIManager>().OpenMenu(building);
    }

    void Deselect()
    {
        FindObjectOfType<BuildingUIManager>().CloseMenu();
    }
}

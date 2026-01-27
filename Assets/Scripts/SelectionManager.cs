using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public LayerMask buildingLayer;
    internal Building building; // used in UnitSpawner

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // the fix for the UI closing on button clicks

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // We only want to hit things on the "Building" layer (Optimization!)
            if (Physics.Raycast(ray, out hit, buildingLayer)) // You can add a LayerMask here later
            {
                // "Try" to see if the thing we hit has the Building script (works for all building parts)
                if (hit.collider.GetComponentInParent<Building>() != null)
                {
                    building = hit.collider.GetComponentInParent<Building>(); // it was Building building
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

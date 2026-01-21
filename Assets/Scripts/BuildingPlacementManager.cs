using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    private GameObject currentBlueprint;
    private GameObject prefabToBuild;
    public LayerMask groundLayer;

    void Update()
    {
        if(currentBlueprint == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, groundLayer))
        {
            currentBlueprint.transform.position = hit.point;

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(prefabToBuild, hit.point, Quaternion.identity);
                Destroy(currentBlueprint);
                currentBlueprint = null; // Stop placing
                
            }
        }
    }

    public void StartPlacing(GameObject buildingPrefab, GameObject buildingBlueprint)
    {
        if (currentBlueprint != null) // destroy blueprint first if already placing something else
        {
            Destroy(currentBlueprint);
        }

        prefabToBuild = buildingPrefab;
        currentBlueprint = Instantiate(buildingBlueprint);
    }
}

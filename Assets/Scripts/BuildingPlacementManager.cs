using System.Collections.Generic;
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

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            currentBlueprint.transform.position = hit.point;

            if (Input.GetMouseButtonDown(0))
            { 
                List<ResourceCost> cost = prefabToBuild.GetComponent<Building>().buildingCost;

                if (FindObjectOfType<GameManager>().TryBuyBuilding(cost))
                {
                    Instantiate(prefabToBuild, hit.point, Quaternion.identity);
                    Destroy(currentBlueprint);
                    currentBlueprint = null;
                }
                
            }
            if(Input.GetMouseButtonDown(1))
            {
                Destroy(currentBlueprint);
                currentBlueprint = null;
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

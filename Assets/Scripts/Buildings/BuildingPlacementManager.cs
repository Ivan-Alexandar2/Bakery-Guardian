using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingPlacementManager : MonoBehaviour
{
    private GameObject currentBlueprint;
    private GameObject prefabToBuild;
    public LayerMask groundLayer;

    [Header("Collision Checks")]
    public LayerMask obstacleLayer;
    public Vector3 buildingHalfExtents = new Vector3(4f, 4f, 4f);

    void Update()
    {
        if(currentBlueprint == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            currentBlueprint.transform.position = hit.point;

            bool isPlacementValid = CanPlaceBuilding();

            if (Input.GetMouseButtonDown(0))
            { 
                List<ResourceCost> cost = prefabToBuild.GetComponent<Building>().buildingCost;

                if (isPlacementValid)
                {
                    if (FindObjectOfType<GameManager>().TryBuyBuilding(cost))
                    {
                        Instantiate(prefabToBuild, hit.point, currentBlueprint.transform.rotation); // Quaternion.identity
                        Destroy(currentBlueprint);
                        currentBlueprint = null;
                    }
                }       
            }
            if(Input.GetMouseButtonDown(1))
            {
                Destroy(currentBlueprint);
                currentBlueprint = null;
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                currentBlueprint.transform.Rotate(0f, 90f, 0f, Space.Self);
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

    private bool CanPlaceBuilding()
    {
        // 1. Find the center of our invisible checking box.
        // If your blueprint's pivot point is at the very bottom (the floor), 
        // we need to shift the box up by its Y-extent so it doesn't sink into the ground.
        Vector3 boxCenter = currentBlueprint.transform.position + new Vector3(0, buildingHalfExtents.y, 0);

        WaterBuilding waterReq = currentBlueprint.GetComponent<WaterBuilding>();

        if (waterReq != null)
        {
            // -- FISHERMAN HUT LOGIC --

            // Subtract the Water layer from the Obstacle layer so it doesn't block itself!
            LayerMask solidObstaclesOnly = obstacleLayer & ~waterReq.waterLayer;

            Debug.Log("AAAAAAAAAAA");

            bool hitsSolid = Physics.CheckBox(boxCenter, buildingHalfExtents, currentBlueprint.transform.rotation, solidObstaclesOnly);
            bool inWater = waterReq.AreAnchorsInWater();

            // It must NOT hit solid buildings, and it MUST be touching water
            if (hitsSolid || !inWater)
            {
                ChangeBlueprintColor(Color.red);
                return false;
            }
        }
        else
        {
            // -- NORMAL BUILDING LOGIC (Bakery, Hospital, etc) --
            // Uses your standard obstacleLayer (which includes water and buildings)
            bool hitsObstacle = Physics.CheckBox(boxCenter, buildingHalfExtents, currentBlueprint.transform.rotation, obstacleLayer);

            if (hitsObstacle)
            {
                ChangeBlueprintColor(Color.red);
                return false;
            }
        }

        // If we made it here, placement is valid!
        ChangeBlueprintColor(Color.green);
        return true;
    }

    private void ChangeBlueprintColor(Color newColor)
    {
        // Get all renderers on the blueprint (in case it has multiple parts)
        Renderer[] renderers = currentBlueprint.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            newColor.a = 0.5f; // Keep it 50% transparent
            rend.material.color = newColor;
        }
    }
}

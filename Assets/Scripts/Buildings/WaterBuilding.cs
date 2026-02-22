using UnityEngine;

public class WaterBuilding : MonoBehaviour
{
    [Header("Water Placement Settings")]
    public Transform[] waterAnchors; // Drag the empty child objects here
    public LayerMask waterLayer;     // Set this to your Water layer
    public float checkRadius = 0.5f; // How big the checking sphere is

    public bool AreAnchorsInWater()
    {
        if (waterAnchors.Length == 0) return false; // Failsafe

        foreach (Transform anchor in waterAnchors)
        {
            // Check if this specific anchor is touching the water layer
            bool touchingWater = Physics.CheckSphere(anchor.position, checkRadius, waterLayer);

            // If even ONE anchor is on dry land, the building is invalid
            if (!touchingWater)
            {
                return false;
            }
        }

        return true; // All anchors are safely in the water!
    }
}

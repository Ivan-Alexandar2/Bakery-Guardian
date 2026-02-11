using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public Image fillImage; // Drag the "Fill" child of the slider here

    [Header("Settings")]
    //public Gradient healthGradient; // Define colors in Inspector (Green -> Red)
    public Vector3 offset = new Vector3(0, 2f, 0); // Height above unit head


    private Transform targetUnit;

    // Called by the Unit when it spawns
    public void Setup(Transform unit, float maxHealth, Color teamColor)
    {
        targetUnit = unit;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;

        // Set the color once
        if (fillImage != null)
            fillImage.color = teamColor;

        // Clean hierarchy 
        GameObject uiFolder = GameObject.Find("WorldUI");
        if (uiFolder == null) uiFolder = new GameObject("WorldUI");

        transform.SetParent(uiFolder.transform);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth;

        // Calculate percentage (0 to 1)
        float percentage = currentHealth / maxHealth;
    }

    void LateUpdate()
    {
        // 1. Follow the unit
        if (targetUnit != null)
        {
            transform.position = targetUnit.position + offset;

            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
        else
        {
            // If unit died, destroy the bar
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("The Data")]
    public Building buildingPrefab; // Drag the specific building prefab here!

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mouse hovered over the button!
        if (buildingPrefab != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(buildingPrefab);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Mouse left the button!
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}

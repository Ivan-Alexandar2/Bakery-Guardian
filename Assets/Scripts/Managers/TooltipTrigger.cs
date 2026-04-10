using UnityEngine;
using UnityEngine.EventSystems;
// useless script
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        // Force the tooltip to hide the moment we click to build!
        if (TooltipManager.Instance != null) TooltipManager.Instance.HideTooltip();
    }
}

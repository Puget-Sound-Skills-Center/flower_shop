using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PreviewClickHandler : MonoBehaviour, IPointerClickHandler
{
    public BouquetDesk desk;                      // assign in inspector or auto-find
    public Image previewImage;                    // assign the preview image or auto-find
    public BouquetPreviewTooltip tooltipHandler;  // optional: assign or auto-find

    private void Start()
    {
        if (desk == null)
            desk = GetComponentInParent<BouquetDesk>();

        if (previewImage == null)
            previewImage = GetComponent<Image>();

        if (tooltipHandler == null)
            tooltipHandler = GetComponent<BouquetPreviewTooltip>() ?? GetComponentInChildren<BouquetPreviewTooltip>();

        if (desk == null)
            Debug.LogWarning("PreviewClickHandler: 'desk' not assigned and not found in parents.");
        if (previewImage == null)
            Debug.LogWarning("PreviewClickHandler: 'previewImage' not assigned and no Image found on this GameObject.");
        if (tooltipHandler == null)
            Debug.Log("PreviewClickHandler: 'tooltipHandler' not assigned. Tooltip won't toggle automatically.");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (desk == null)
        {
            Debug.LogWarning("PreviewClickHandler.OnPointerClick: desk is null, ignoring click.");
            return;
        }

        if (desk.currentStage != BouquetDesk.Stage.Review)
            return;

        if (previewImage != null)
        {
            var c = previewImage.color;
            c.a = 0f; // fully transparent
            previewImage.color = c;
        }
        else
        {
            Debug.LogWarning("PreviewClickHandler.OnPointerClick: previewImage is null; cannot change alpha.");
        }

        // If there's a tooltip handler, toggle it (open/close). This avoids NullReferenceExceptions.
        if (tooltipHandler != null)
        {
            tooltipHandler.ToggleTooltip();
        }
    }
}

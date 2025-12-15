using UnityEngine;

public class TooltipButtonTrigger : MonoBehaviour
{
    public BouquetPreviewTooltip previewTooltip;

    public void OnButtonClick()
    {
        Debug.Log("BUTTON CLICKED");

        if (previewTooltip == null)
        {
            Debug.LogError("previewTooltip is NULL");
            return;
        }

        previewTooltip.ToggleTooltip();
    }

}

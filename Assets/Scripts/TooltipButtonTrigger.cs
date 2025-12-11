using UnityEngine;

public class TooltipButtonTrigger : MonoBehaviour
{
    public BouquetPreviewTooltip previewTooltip;

    public void OnButtonClick()
    {
        if (previewTooltip != null)
            previewTooltip.ToggleTooltip();
    }
}

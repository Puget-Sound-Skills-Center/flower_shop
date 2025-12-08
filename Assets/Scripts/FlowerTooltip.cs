using TMPro;
using UnityEngine;

public class FlowerTooltip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void Setup(FlowerData flower)
    {
        if (flower == null) return;

        if (nameText != null)
            nameText.text = flower.flowerName;

        if (descriptionText != null)
            descriptionText.text = flower.tooltipDescription; // or another field
    }
}

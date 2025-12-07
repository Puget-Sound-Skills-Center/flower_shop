using UnityEngine;
using UnityEngine.UI;

public class ShelfBouquetButton : MonoBehaviour
{
    public FlowerData flowerData;

    private Button button;
    private Image outline;

    public bool isSelected = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        outline = GetComponent<Image>(); // or add a child outline image

        button.onClick.AddListener(ToggleSelect);
    }

    private void ToggleSelect()
    {
        isSelected = !isSelected;

        // visual selection highlight
        if (outline != null)
            outline.color = isSelected ? new Color(1f, 1f, 0.4f, 1f) : Color.white;

        // inform manager
        MultiSellManager.Instance.UpdateSelection(this);
    }
}

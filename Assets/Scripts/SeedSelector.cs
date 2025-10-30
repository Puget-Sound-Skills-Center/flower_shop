using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Flower Data")]
    public FlowerData flowerToSelect;

    [Header("Highlight")]
    public Image highlightImage; // Assign the highlight image in Inspector

    private void Awake()
    {
        if (highlightImage != null)
            highlightImage.enabled = false; // Start hidden
    }

    // Hover highlight ON
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.enabled = true;
    }

    // Hover highlight OFF (unless selected)
    public void OnPointerExit(PointerEventData eventData)
    {
        var gm = GameManager.Instance;
        if (highlightImage == null) return;

        // Safely check selectedFlower (GameManager may be null in tests / editor)
        if (gm == null || gm.selectedFlower != flowerToSelect)
            highlightImage.enabled = false;
    }

    // Click to select this seed
    public void OnPointerClick(PointerEventData eventData)
    {
        if (flowerToSelect == null)
        {
            Debug.LogWarning($"SeedSelector on '{gameObject.name}' has no FlowerData assigned.");
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("SeedSelector: GameManager instance missing!");
            return;
        }

        // Set selected flower and update UI
        gm.selectedFlower = flowerToSelect;
        gm.UpdateSelectedFlowerUI();

        Debug.Log($"Selected seed: {flowerToSelect.name} (selectedFlower set in GameManager)");

        // Update highlight visuals: only this selector should show highlight
        SeedSelector[] allSelectors = FindObjectsOfType<SeedSelector>();
        foreach (SeedSelector selector in allSelectors)
        {
            if (selector.highlightImage != null)
                selector.highlightImage.enabled = (selector == this);
        }
    }
}

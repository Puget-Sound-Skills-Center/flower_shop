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
        if (highlightImage != null && GameManager.Instance.selectedFlower != flowerToSelect)
            highlightImage.enabled = false;
    }

    // Click to select this seed
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance missing!");
            return;
        }

        GameManager.Instance.selectedFlower = flowerToSelect;
        GameManager.Instance.UpdateSelectedFlowerUI();

        Debug.Log("Selected seed: " + flowerToSelect.name);

        // Turn off other highlights
        SeedSelector[] allSelectors = FindObjectsOfType<SeedSelector>();
        foreach (SeedSelector selector in allSelectors)
        {
            if (selector.highlightImage != null)
                selector.highlightImage.enabled = (selector == this);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SeedSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FlowerData flower;    // The flower this button represents
    public Button button;        // The UI button
    public Image highlightImage; // Optional: assign in Inspector for selection highlight

    private bool isSelected = false;

    private void Start()
    {
        if (button == null)
        {
            Debug.LogWarning("SeedSelector: Button reference is missing.");
            return;
        }
        if (flower == null)
        {
            Debug.LogWarning("SeedSelector: FlowerData reference is missing.");
            button.interactable = false;
            return;
        }
        button.onClick.AddListener(OnSelectFlower);

        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnSelectFlower);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && highlightImage != null)
            highlightImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && highlightImage != null)
            highlightImage.enabled = false;
    }

    private void OnSelectFlower()
    {
        if (flower == null)
        {
            Debug.LogWarning("SeedSelector: No flower assigned for selection.");
            return;
        }
        if (GameManager.Instance == null)
        {
            Debug.LogError("SeedSelector: GameManager instance is missing.");
            return;
        }

        GameManager.Instance.selectedFlower = flower;

        // If you have a UI update method, call it
        GameManager.Instance.UpdateSelectedFlowerUI();

        // Highlight logic: disable all, enable this
        SeedSelector[] allSelectors = FindObjectsOfType<SeedSelector>();
        foreach (SeedSelector selector in allSelectors)
        {
            selector.SetSelected(false);
        }
        SetSelected(true);

        Debug.Log("Selected flower: " + flower.flowerName);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (highlightImage != null)
            highlightImage.enabled = selected;
    }
}

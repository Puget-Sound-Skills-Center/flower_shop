using UnityEngine;
using UnityEngine.EventSystems;

public class BouquetPreviewTooltip : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject currentTooltip;
    private BouquetDesk desk;
    private UICursorFollower cursor;

    [Header("Tooltip Prefab (assign manually)")]
    public GameObject tooltipPrefab;   // <-- NEW: assign in inspector

    [Header("Highlight")]
    public GameObject highlightFrame;  // optional

    private void Start()
    {
        desk = GetComponentInParent<BouquetDesk>();
        cursor = FindObjectOfType<UICursorFollower>();

        if (highlightFrame != null)
            highlightFrame.SetActive(false);
    }

    // ------------------ HOVER HIGHLIGHT ------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (desk != null && desk.currentStage == BouquetDesk.Stage.Review)
        {
            if (highlightFrame != null)
                highlightFrame.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightFrame != null)
            highlightFrame.SetActive(false);
    }

    // ------------------ CLICK → SHOW TOOLTIP ------------------

    public void OnPointerClick(PointerEventData eventData)
    {
        // Only allow tooltips during Review stage
        if (desk == null || desk.currentStage != BouquetDesk.Stage.Review)
            return;

        var flower = desk.SelectedFlower;
        if (flower == null)
            return;

        // Close old tooltip if open
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
            return;
        }

        // Must assign a prefab manually
        if (tooltipPrefab == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No tooltip prefab assigned in inspector!");
            return;
        }

        Canvas canvas = desk.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No parent Canvas found!");
            return;
        }

        // Spawn tooltip
        currentTooltip = Instantiate(tooltipPrefab, canvas.transform);
        RectTransform tipRect = currentTooltip.GetComponent<RectTransform>();

        // Position near custom UI cursor if available
        if (cursor != null)
            tipRect.anchoredPosition = cursor.GetCursorPosition() + new Vector2(25f, -25f);
        else
            tipRect.position = Input.mousePosition;

        // Fill tooltip content
        var tooltip = currentTooltip.GetComponent<FlowerTooltip>();
        if (tooltip != null)
            tooltip.Setup(flower);
    }
}

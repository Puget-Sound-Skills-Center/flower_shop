using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BouquetPreviewTooltip : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject currentTooltip;
    private BouquetDesk desk;
    private UICursorFollower cursor;

    [Header("Tooltip Prefab (assign manually)")]
    public GameObject tooltipPrefab;   // <-- assign in inspector

    [Header("Highlight")]
    public GameObject highlightFrame;  // optional

    private void Start()
    {
        desk = GetComponentInParent<BouquetDesk>();
        cursor = FindObjectOfType<UICursorFollower>();

        // Ensure this UI element can receive pointer events.
        var img = GetComponent<Image>();
        if (img == null)
        {
            img = gameObject.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f); // fully transparent
            img.raycastTarget = true;
        }
        else
        {
            img.raycastTarget = true;
        }

        if (highlightFrame != null)
            highlightFrame.SetActive(false);
    }

    private void OnDisable()
    {
        CloseTooltip();
        if (highlightFrame != null)
            highlightFrame.SetActive(false);
    }

    private void Update()
    {
        // Close tooltip automatically if desk stage leaves Review (prevents stale tooltip)
        if (IsTooltipOpen() && (desk == null || desk.currentStage != BouquetDesk.Stage.Review))
        {
            CloseTooltip();
        }
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
        ToggleTooltip();
    }

    // Public helper so other scripts (e.g. PreviewClickHandler) can open/close the tooltip programmatically.
    public void ToggleTooltip()
    {
        // Only allow tooltips during Review stage
        if (desk == null || desk.currentStage != BouquetDesk.Stage.Review)
            return;

        var flower = desk.SelectedFlower;
        if (flower == null)
            return;

        // Toggle: Close old tooltip if open
        if (IsTooltipOpen())
        {
            CloseTooltip();
            return;
        }

        OpenTooltip(flower);
    }

    private void OpenTooltip(FlowerData flower)
    {
        // Must assign a prefab manually
        if (tooltipPrefab == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No tooltip prefab assigned in inspector!");
            return;
        }

        // Find Canvas to parent tooltip under
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No Canvas found to parent tooltip!");
            return;
        }

        // Spawn tooltip
        currentTooltip = Instantiate(tooltipPrefab, canvas.transform);
        RectTransform tipRect = currentTooltip.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Position: prefer custom UI cursor position when available, otherwise convert screen point.
        if (cursor != null)
        {
            tipRect.anchoredPosition = cursor.GetCursorPosition() + new Vector2(25f, -25f);
        }
        else
        {
            Vector2 localPoint;
            bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint);

            if (ok)
                tipRect.anchoredPosition = localPoint + new Vector2(25f, -25f);
            else
                tipRect.position = Input.mousePosition; // fallback
        }

        // Fill tooltip content
        var tooltip = currentTooltip.GetComponent<FlowerTooltip>();
        if (tooltip != null)
            tooltip.Setup(flower);
    }

    private void CloseTooltip()
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
        }
    }

    private bool IsTooltipOpen()
    {
        return currentTooltip != null;
    }
}

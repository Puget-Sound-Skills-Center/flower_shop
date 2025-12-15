using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BouquetPreviewTooltip : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    private GameObject currentTooltip;
    [Header("Assign this directly (do not rely on GetComponentInParent)")]
    public BouquetDesk desk;                       // assign in Inspector
    private UICursorFollower cursor;

    [Header("Tooltip Prefab")]
    public GameObject tooltipPrefab;

    [Header("Highlight Frame (optional)")]
    public GameObject highlightFrame;

    private void Start()
    {
        // Prefer explicit assignment in inspector. If missing, attempt a fallback lookup but warn.
        if (desk == null)
        {
            desk = GetComponentInParent<BouquetDesk>();
            if (desk != null)
                Debug.LogWarning("BouquetPreviewTooltip: 'desk' was not assigned in Inspector — auto-found via GetComponentInParent. Assign BouquetDesk in the Inspector to avoid hierarchy dependence.");
            else
                Debug.LogWarning("BouquetPreviewTooltip: 'desk' not assigned in Inspector and not found in parents. Assign BouquetDesk on this component in the Inspector.");
        }

        cursor = FindObjectOfType<UICursorFollower>();

        // Ensure this area can receive hover raycasts (editor should ideally have a Graphic).
        var img = GetComponent<Image>();
        if (img == null)
        {
            img = gameObject.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f);
        }
        img.raycastTarget = true;

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
        if (IsTooltipOpen() &&
            (desk == null || desk.currentStage != BouquetDesk.Stage.Review))
        {
            CloseTooltip();
        }
    }

    // ------------------- Hover Highlight -------------------

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

    // ------------------- Called by Hidden Button -------------------

    public void ToggleTooltip()
    {
        if (desk == null)
        {
            Debug.LogWarning("BouquetPreviewTooltip.ToggleTooltip: 'desk' is null. Assign BouquetDesk in the Inspector.");
            return;
        }

        if (desk.currentStage != BouquetDesk.Stage.Review)
            return;

        var flower = desk.SelectedFlower;
        if (flower == null)
            return;

        if (IsTooltipOpen())
            CloseTooltip();
        else
            OpenTooltip(flower);
    }

    private void OpenTooltip(FlowerData flower)
    {
        if (tooltipPrefab == null)
        {
            Debug.LogError("BouquetPreviewTooltip: Missing tooltipPrefab!");
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No Canvas found!");
            return;
        }

        currentTooltip = Instantiate(tooltipPrefab, canvas.transform);
        RectTransform tipRect = currentTooltip.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        if (cursor != null)
        {
            tipRect.anchoredPosition =
                cursor.GetCursorPosition() + new Vector2(25f, -25f);
        }
        else
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint);

            tipRect.anchoredPosition = localPoint + new Vector2(25f, -25f);
        }

        var tooltip = currentTooltip.GetComponent<FlowerTooltip>();
        if (tooltip != null)
            tooltip.Setup(flower);
    }

    private void CloseTooltip()
    {
        if (currentTooltip != null)
        {
            var anim = currentTooltip.GetComponent<TooltipFadeUp>();
            if (anim != null)
                anim.PlayFadeOutAndDestroy();
            else
                Destroy(currentTooltip);

            currentTooltip = null;
        }
    }

    private bool IsTooltipOpen()
    {
        return currentTooltip != null;
    }
}

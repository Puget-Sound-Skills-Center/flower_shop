using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows a seed UI element to be dragged and dropped.
/// </summary>
public class DraggableSeed : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 startPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            Debug.LogError("CanvasGroup component missing on DraggableSeed.");
        if (rectTransform == null)
            Debug.LogError("RectTransform component missing on DraggableSeed.");
    }

    /// <summary>
    /// Called when drag begins.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Called during dragging.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null)
            return;

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, Input.mousePosition, eventData.pressEventCamera, out globalMousePos))
        {
            rectTransform.position = globalMousePos;
        }
    }

    /// <summary>
    /// Called when drag ends.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (rectTransform != null)
            rectTransform.position = startPosition;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }
}

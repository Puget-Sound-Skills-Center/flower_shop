using UnityEngine;
using UnityEngine.UI;

public class UICursorFollower : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float followSpeed = 30f;

    private RectTransform cursorRect;
    private RectTransform canvasRect;

    void Awake()
    {
        cursorRect = cursorImage.rectTransform;
        canvasRect = targetCanvas.GetComponent<RectTransform>();

        cursorImage.raycastTarget = false;
        cursorImage.enabled = true; // ✅ ALWAYS ON

        Cursor.visible = false;     // Optional: hide system cursor
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        Camera cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : targetCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            cam,
            out Vector2 localPos
        );

        Vector2 target = localPos + offset;

        cursorRect.anchoredPosition =
            Vector2.Lerp(cursorRect.anchoredPosition, target, Time.unscaledDeltaTime * followSpeed);

        // Click feedback only
        if (Input.GetMouseButtonDown(0))
            cursorRect.localScale = Vector3.one * 0.85f;

        if (Input.GetMouseButtonUp(0))
            cursorRect.localScale = Vector3.one;
    }
}

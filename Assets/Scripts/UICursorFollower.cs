using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class UICursorFollower : MonoBehaviour
{
    [Header("Cursor Setup")]
    public Image cursorImage;                 // UI Image for cursor sprite
    public Canvas uiCanvas;                   // Canvas reference
    public bool hideSystemCursor = true;      // Hide OS cursor

    [Header("Cursor Behavior")]
    public Vector2 offset = Vector2.zero;     // pixel offset
    [Range(0f, 50f)] public float followSpeed = 20f;  // 0 = instant, lower = smoother

    private RectTransform cursorRect;
    private RectTransform canvasRect;

    void Awake()
    {
        if (uiCanvas == null)
            uiCanvas = GetComponent<Canvas>();

        if (cursorImage == null)
        {
            Debug.LogError("UICursorFollower: Please assign a UI Image for cursorImage.");
            enabled = false;
            return;
        }

        cursorRect = cursorImage.rectTransform;
        canvasRect = uiCanvas.GetComponent<RectTransform>();
        cursorImage.raycastTarget = false; // don’t block UI clicks
    }

    void OnEnable()
    {
        if (hideSystemCursor)
            Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 localPoint;

        // Convert to local canvas coordinates
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera, out localPoint))
        {
            Vector2 target = localPoint + offset;
            if (followSpeed <= 0f)
                cursorRect.anchoredPosition = target;
            else
                cursorRect.anchoredPosition = Vector2.Lerp(cursorRect.anchoredPosition, target, Time.deltaTime * followSpeed);
        }

        // Optional click pulse
        if (Input.GetMouseButtonDown(0))
            cursorRect.localScale = Vector3.one * 0.85f;
        if (Input.GetMouseButtonUp(0))
            cursorRect.localScale = Vector3.one;
    }

    // Optional helpers
    public void SetCursorSprite(Sprite sprite, Vector2 pivot)
    {
        cursorImage.sprite = sprite;
        cursorRect.pivot = pivot;
    }

    public Vector2 GetCursorPosition()
    {
        return cursorRect.anchoredPosition;
    }

    public void SetCursorVisible(bool visible)
    {
        cursorImage.enabled = visible;
        Cursor.visible = !visible ? false : Cursor.visible;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class BouquetPreviewTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject currentTooltip;
    private BouquetDesk desk;

    [Header("Optional offset from mouse")]
    public Vector2 tooltipOffset = new Vector2(15f, -15f);

    private void Awake()
    {
        // Ensure we get the BouquetDesk reference from parent
        desk = GetComponentInParent<BouquetDesk>();
        if (desk == null)
            Debug.LogError("BouquetPreviewTooltip: Could not find BouquetDesk in parent hierarchy.");
    }

    private void Update()
    {
        // Make tooltip follow the mouse
        if (currentTooltip != null)
        {
            Vector3 mousePos = Input.mousePosition;
            currentTooltip.transform.position = mousePos + (Vector3)tooltipOffset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (desk == null || desk.SelectedFlower == null) return;

        var flower = desk.SelectedFlower;

        if (flower.tooltipPrefab == null)
        {
            Debug.LogWarning($"No tooltip prefab assigned for {flower.flowerName}");
            return;
        }

        // Spawn tooltip as child of canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BouquetPreviewTooltip: No Canvas found in scene!");
            return;
        }

        currentTooltip = Instantiate(flower.tooltipPrefab, canvas.transform);
        currentTooltip.transform.position = Input.mousePosition + (Vector3)tooltipOffset;

        // Setup tooltip info if it has FlowerTooltip script
        var tooltip = currentTooltip.GetComponent<FlowerTooltip>();
        if (tooltip != null)
            tooltip.Setup(flower);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
        }
    }
}

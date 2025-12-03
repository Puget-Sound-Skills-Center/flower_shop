using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShelfBouquetButton : MonoBehaviour
{
    [Header("Assigned at runtime")]
    [SerializeField] private FlowerData flowerData;
    [SerializeField] private ShopShelf shopShelf;

    [Header("Pricing")]
    public int sellPrice = 30;

    private Button button;

    // Public accessors
    public FlowerData FlowerData => flowerData;
    public ShopShelf Shelf => shopShelf;

    private bool initialized = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        // Initially disable interaction if not yet initialized
        if (button != null)
            button.interactable = false;
    }

    /// <summary>
    /// Call this immediately after Instantiate(...) from ShopShelf.
    /// </summary>
    public void Initialize(FlowerData flower, ShopShelf parentShelf, int price = 30)
    {
        flowerData = flower;
        shopShelf = parentShelf;
        sellPrice = price;

        if (button == null) button = GetComponent<Button>();

        // Make sure we don't double-add listeners
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickSell);

        // now enable button
        button.interactable = true;
        initialized = true;
    }

    private void OnClickSell()
    {
        if (!initialized)
        {
            Debug.LogWarning($"ShelfBouquetButton {gameObject.name} clicked but not initialized. Ignoring.");
            return;
        }

        if (flowerData == null)
        {
            Debug.LogError("ShelfBouquetButton: NO FlowerData assigned!");
            return;
        }

        // delegate to GameManager (keeps responsibilities separated)
        GameManager.Instance.SellBouquetFromButton(this);
    }
}

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShelfBouquetButton : MonoBehaviour
{
    private Button button;

    // This will be filled by ShopShelf when the bouquet is spawned
    public FlowerData flowerData;

    [Header("UI Reference")]
    public Image bouquetImage; // assign in prefab

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickSell);

        // Set correct bouquet image
        if (flowerData != null && bouquetImage != null)
        {
            bouquetImage.sprite = flowerData.bouquetFinalSprite;
            bouquetImage.enabled = true;
        }
    }

    private void OnClickSell()
    {
        if (flowerData == null)
        {
            Debug.LogError("ShelfBouquetButton has NO FlowerData assigned!");
            return;
        }

        // Sell from GameManager using the correct reference
        bool success = GameManager.Instance.SellBouquet(flowerData, pricePerBouquet: 25);

        if (success)
        {
            Debug.Log($"Sold bouquet of {flowerData.flowerName}");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Trying to sell a bouquet that does not exist in inventory.");
        }
    }
}

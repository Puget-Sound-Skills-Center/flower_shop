using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopShelf : MonoBehaviour
{
    [Header("Shelf Settings")]
    public Transform shelfArea;                // Assign in Inspector (e.g. a UI panel)
    public GameObject bouquetDisplayPrefab;    // Prefab for each bouquet display

    private void Start()
    {
        RefreshShelf();
    }

    public void RefreshShelf()
    {
        if (shelfArea == null || bouquetDisplayPrefab == null)
        {
            Debug.LogWarning("ShopShelf: Missing shelfArea or bouquetDisplayPrefab reference!");
            return;
        }

        // Clear previous bouquets
        foreach (Transform child in shelfArea)
            Destroy(child.gameObject);

        // Load bouquets from GameManager
        Dictionary<FlowerData, int> bouquetInventory = GameManager.Instance.GetBouquetInventory();

        foreach (var kvp in bouquetInventory)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                GameObject bouquetObj = Instantiate(bouquetDisplayPrefab, shelfArea);
                var img = bouquetObj.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = kvp.Key.readySprite;
                    img.enabled = true;
                }
            }
        }

        Debug.Log($"ShopShelf: Refreshed with {bouquetInventory.Count} bouquet types.");
    }

    public void AddBouquetToShelf(FlowerData flower)
    {
        GameManager.Instance.AddBouquet(flower);
        RefreshShelf();
    }
}

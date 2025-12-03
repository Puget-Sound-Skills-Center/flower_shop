using UnityEngine;
using System.Collections.Generic;

public class ShopShelf : MonoBehaviour
{
    [Header("Shelf Settings")]
    public Transform shelfArea;
    public GameObject bouquetDisplayPrefab;

    // runtime list of bouquets shown on the shelf (each entry = one display)
    private List<FlowerData> shelfBouquets = new List<FlowerData>();

    private void Start()
    {
        RefreshShelf();
    }

    public void RefreshShelf()
    {
        if (shelfArea == null || bouquetDisplayPrefab == null)
        {
            Debug.LogError("ShopShelf missing shelfArea or bouquetDisplayPrefab!");
            return;
        }

        Debug.Log($"ShopShelf.RefreshShelf() called. shelfBouquets.Count = {shelfBouquets.Count}");

        // Clear old visual children first
        foreach (Transform child in shelfArea)
            Destroy(child.gameObject);

        // Instantiate a button prefab for each tracked bouquet
        for (int i = 0; i < shelfBouquets.Count; i++)
        {
            FlowerData flower = shelfBouquets[i];
            if (flower == null)
            {
                Debug.LogWarning($"ShopShelf: shelfBouquets[{i}] is null. Skipping.");
                continue;
            }

            GameObject obj = Instantiate(bouquetDisplayPrefab, shelfArea);
            obj.name = $"BouquetDisplay_{flower.flowerName}_{i}";

            var button = obj.GetComponent<ShelfBouquetButton>();
            if (button == null)
            {
                Debug.LogError("bouquetDisplayPrefab is missing ShelfBouquetButton script!");
                continue;
            }

            // initialize runtime data (price optional)
            button.Initialize(flower, this);

            // set the image sprite if prefab has an Image (optional convenience)
            var img = obj.GetComponent<UnityEngine.UI.Image>();
            if (img != null && flower.readySprite != null)
            {
                img.sprite = flower.readySprite;
                img.enabled = true;
            }
        }
    }

    /// <summary>
    /// Adds a bouquet to the tracked list (one entry = one visible item)
    /// and refreshes visuals.
    /// Call this when a bouquet is finished/wrapped and sent to shop.
    /// </summary>
    public void AddBouquetToShelf(FlowerData flower)
    {
        if (flower == null) return;

        shelfBouquets.Add(flower);
        Debug.Log($"ShopShelf.AddBouquetToShelf: added {flower.flowerName}. total on shelf now: {shelfBouquets.Count}");
        RefreshShelf();
    }

    /// <summary>
    /// Remove a specific display button from the shelf (called after sale).
    /// Removes only the first matching instance.
    /// </summary>
    public void RemoveBouquetFromShelf(ShelfBouquetButton button)
    {
        if (button == null) return;

        FlowerData flower = button.FlowerData;
        if (flower != null)
        {
            int idx = shelfBouquets.FindIndex(f => f == flower);
            if (idx >= 0)
            {
                shelfBouquets.RemoveAt(idx);
                Debug.Log($"ShopShelf.RemoveBouquetFromShelf: removed {flower.flowerName} at index {idx}");
            }
            else
            {
                // fallback: try removing by reference to the exact GameObject (less ideal)
                Debug.LogWarning("ShopShelf: Could not find matching FlowerData in shelfBouquets when removing. Removing visuals anyway.");
            }
        }

        // Refresh UI which will also destroy old visual buttons
        RefreshShelf();
    }
}

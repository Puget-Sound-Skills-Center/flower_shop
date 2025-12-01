using UnityEngine;
using System.Collections.Generic;

public class ShopShelf : MonoBehaviour
{
    [Header("Shelf Settings")]
    public Transform shelfArea;
    public GameObject bouquetDisplayPrefab;

    private List<FlowerData> shelfBouquets = new List<FlowerData>();

    /// <summary>
    /// Refresh the shelf UI to match the current shelfBouquets list.
    /// </summary>
    public void RefreshShelf()
    {
        if (shelfArea == null || bouquetDisplayPrefab == null) return;

        // Clear old buttons
        foreach (Transform child in shelfArea)
            Destroy(child.gameObject);

        // Instantiate new buttons dynamically
        foreach (var flower in shelfBouquets)
        {
            GameObject bouquetObj = Instantiate(bouquetDisplayPrefab, shelfArea);

            // Get required components
            ShelfBouquetButton buttonScript = bouquetObj.GetComponent<ShelfBouquetButton>();
            UnityEngine.UI.Button uiButton = bouquetObj.GetComponent<UnityEngine.UI.Button>();

            if (buttonScript != null && uiButton != null)
            {
                // Assign flower and shelf reference dynamically
                buttonScript.flowerData = flower;
                buttonScript.shopShelf = this;

                // Remove any old listeners to avoid duplicates
                uiButton.onClick.RemoveAllListeners();

                // Assign GameManager method to OnClick dynamically
                uiButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.SellBouquetFromButton(buttonScript);
                });
            }
            else
            {
                Debug.LogWarning("Bouquet prefab missing ShelfBouquetButton or Button component!");
            }
        }
    }


    /// <summary>
    /// Add a bouquet to the shelf and refresh UI
    /// </summary>
    public void AddBouquetToShelf(FlowerData flower)
    {
        if (flower == null) return;

        shelfBouquets.Add(flower);
        RefreshShelf();
    }

    /// <summary>
    /// Remove bouquet from shelf using button reference
    /// </summary>
    public void RemoveBouquetFromShelf(ShelfBouquetButton button)
    {
        if (button == null) return;

        FlowerData flower = button.GetFlowerData();
        if (flower != null && shelfBouquets.Contains(flower))
            shelfBouquets.Remove(flower);

        Destroy(button.gameObject);
    }
}

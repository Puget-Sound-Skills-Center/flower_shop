using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ShopShelf : MonoBehaviour
{
    [Header("Shelf Settings")]
    public Transform shelfArea;
    public GameObject bouquetDisplayPrefab;

    private List<FlowerData> shelfBouquets = new List<FlowerData>();

    public void RefreshShelf()
    {

        Debug.Log("Refreshing shelf. Items: " + shelfBouquets.Count);

        foreach (var f in shelfBouquets)
        {
            Debug.Log("Item in shelf: " + (f == null ? "NULL" : f.name));
        }

        if (shelfArea == null || bouquetDisplayPrefab == null)
        {
            Debug.LogError("ShopShelf missing shelfArea or bouquetDisplayPrefab!");
            return;
        }

        // Destroy old buttons
        foreach (Transform child in shelfArea)
            Destroy(child.gameObject);

        // Spawn buttons
        foreach (var flower in shelfBouquets)
        {
            GameObject obj = Instantiate(bouquetDisplayPrefab, shelfArea);

            ShelfBouquetButton buttonScript = obj.GetComponent<ShelfBouquetButton>();

            if (buttonScript == null)
            {
                Debug.LogError("Prefab missing ShelfBouquetButton!");
                continue;
            }

            // Assign runtime data
            buttonScript.flowerData = flower;
            buttonScript.shopShelf = this;

            // OnClick is now handled INSIDE the prefab component
        }
    }




    public void AddBouquetToShelf(FlowerData flower)
    {
        if (flower == null) return;

        shelfBouquets.Add(flower);
        RefreshShelf();
    }

    public void RemoveBouquetFromShelf(ShelfBouquetButton button)
    {
        if (button == null) return;

        FlowerData flower = button.GetFlowerData();
        if (flower != null && shelfBouquets.Contains(flower))
            shelfBouquets.Remove(flower);
        RefreshShelf();
        Destroy(button.gameObject);
    }

    public static implicit operator ShopShelf(Transform v)
    {
        throw new NotImplementedException();
    }
}

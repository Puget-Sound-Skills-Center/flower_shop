using UnityEngine;

public class ShopShelf : MonoBehaviour
{
    public Transform shelfArea;
    public GameObject bouquetDisplayPrefab;

    public void AddBouquetToShelf(FlowerData flower)
    {
        if (flower == null)
        {
            Debug.LogWarning("ShopShelf.AddBouquetToShelf: flower is null");
            return;
        }

        if (bouquetDisplayPrefab == null || shelfArea == null)
        {
            Debug.LogError("ShopShelf missing references!");
            return;
        }

        GameObject display = Instantiate(bouquetDisplayPrefab, shelfArea);
        display.name = $"Bouquet_{flower.flowerName}";

        var button = display.GetComponent<ShelfBouquetButton>();
        if (button != null)
        {
            button.flowerData = flower; // assign correct FlowerData
        }
    }

}


// ShelfBouquetButton.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShelfBouquetButton : MonoBehaviour
{
    public FlowerData flowerData;  // Assigned dynamically by ShopShelf
    public int sellPrice = 50;     // Default price
    public ShopShelf shopShelf;    // Assigned dynamically

    public FlowerData GetFlowerData() => flowerData;
}

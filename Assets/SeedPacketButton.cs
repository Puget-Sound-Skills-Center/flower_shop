using UnityEngine;

public class SeedPacketButton : MonoBehaviour
{
    public FlowerData flowerData;       // assign in Inspector
    public Transform popupSpawnPoint;   // where to spawn confirmation (usually this button itself)

    private SeedShop shop;

    private void Awake()
    {
        shop = FindObjectOfType<SeedShop>();
        if (shop == null)
            Debug.LogError("SeedShop not found in scene!");
    }

    // This method is safe for Unity OnClick (no params)
    public void OnBuyButtonClicked()
    {
        if (shop != null && flowerData != null)
        {
            shop.ShowBuyConfirmation(flowerData, popupSpawnPoint != null ? popupSpawnPoint : transform);
        }
        else
        {
            Debug.LogError("SeedPacketButton missing FlowerData or SeedShop reference!");
        }
    }
}

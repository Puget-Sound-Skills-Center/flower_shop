using UnityEngine;
using System.Collections.Generic;

public class PotsTab : MonoBehaviour
{
    [Header("Prefabs & Layout")]
    public Transform potGridParent;         // Parent to hold pot prefabs
    public PotShopItem[] potPrefabs;        // Assign each unique prefab here

    [Header("Bundle Settings")]
    public string[] potNames = { "Small Pot Bundle", "Medium Pot Bundle", "Large Pot Bundle", "Mega Pot Bundle" };
    public int[] potAmounts = { 5, 10, 20, 50 };
    public int[] potPrices = { 10, 20, 35, 75 };

    private readonly List<PotShopItem> potItems = new();

    private void OnEnable()
    {
        SpawnPotItems();
        RefreshOwnedCounts();
    }

    private void SpawnPotItems()
    {
        // Clean old children
        foreach (Transform child in potGridParent)
            Destroy(child.gameObject);

        potItems.Clear();

        if (potGridParent == null || potPrefabs == null || potPrefabs.Length == 0)
        {
            Debug.LogWarning("PotsTab: Missing references!");
            return;
        }

        int count = Mathf.Min(potPrefabs.Length, potNames.Length, potAmounts.Length, potPrices.Length);

        for (int i = 0; i < count; i++)
        {
            PotShopItem prefab = potPrefabs[i];
            PotShopItem item = Instantiate(prefab, potGridParent);

            // Assign bundle data directly
            item.potDisplayName = potNames[i];
            item.potsOnSale = potAmounts[i];
            item.price = potPrices[i];

            // Set initial owned count
            item.SetOwned(GameManager.Instance != null ? GameManager.Instance.GetPots() : 0);

            // Refresh the UI
            item.UpdateUI();

            // Hook callback for dynamic updates
            item.OnOwnedChanged = RefreshOwnedCounts;

            potItems.Add(item);
        }
    }

    private void RefreshOwnedCounts()
    {
        int ownedCount = GameManager.Instance != null ? GameManager.Instance.GetPots() : 0;

        foreach (var item in potItems)
            item.SetOwned(ownedCount);
    }
}

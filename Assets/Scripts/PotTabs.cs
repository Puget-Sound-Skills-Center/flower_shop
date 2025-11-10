using UnityEngine;
using System.Collections.Generic;

public class PotsTab : MonoBehaviour
{
    [Header("Prefabs & Layout")]
    public Transform potGridParent; // Parent to hold pot prefabs
    public PotShopItem[] potPrefabs; // Assign your prefabs here (Small, Medium, Large, etc.)

    [Header("Bundle Settings")]
    public string[] potNames = { "Small Pot Bundle", "Medium Pot Bundle", "Large Pot Bundle", "Mega Pot Bundle" };
    public int[] potAmounts = { 5, 10, 20, 50 };  // Pots per bundle
    public int[] potPrices = { 10, 20, 35, 75 };  // Price per bundle

    private readonly List<PotShopItem> potItems = new();

    private void OnEnable()
    {
        InitializePotItems();
        RefreshOwnedCounts();
    }

    private void InitializePotItems()
    {
        // Clear old items
        foreach (Transform child in potGridParent)
        {
            Destroy(child.gameObject);
        }
        potItems.Clear();

        // Safety checks
        if (potGridParent == null)
        {
            Debug.LogWarning("PotsTab: potGridParent not assigned!");
            return;
        }
        if (potPrefabs == null || potPrefabs.Length == 0)
        {
            Debug.LogWarning("PotsTab: No pot prefabs assigned!");
            return;
        }

        int count = Mathf.Min(potPrefabs.Length, potNames.Length, potAmounts.Length, potPrices.Length);

        for (int i = 0; i < count; i++)
        {
            // Instantiate prefab into the parent container
            PotShopItem prefab = potPrefabs[i];
            PotShopItem item = Instantiate(prefab, potGridParent);

            // Assign data
            item.potDisplayName = potNames[i];
            item.potsOnSale = potAmounts[i];
            item.price = potPrices[i];

            // Initialize UI
            item.UpdateUI();

            // Hook event for refreshing ownership
            item.OnOwnedChanged = RefreshOwnedCounts;

            // Add to tracking list
            potItems.Add(item);
        }
    }

    private void RefreshOwnedCounts()
    {
        int ownedCount = GameManager.Instance != null ? GameManager.Instance.GetPots() : 0;

        foreach (var item in potItems)
        {
            item.SetOwned(ownedCount);
        }
    }
}

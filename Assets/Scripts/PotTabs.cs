using UnityEngine;
using System.Collections.Generic;

public class PotsTab : MonoBehaviour
{
    [Header("Prefabs & Layout")]
    public Transform potGridParent;         // Parent to hold pot prefabs
    public PotShopItem[] potPrefabs;        // Assign each unique prefab here

    [Header("Shared Dependencies")]
    public GrowingAreaManager growingAreaManager;
    public GameObject growingPotPrefab;


    [Header("Bundle Settings")]
    public string[] potNames = { "Small Pot Bundle", "Medium Pot Bundle", "Large Pot Bundle", "Mega Pot Bundle" };
    public int[] potAmounts = { 5, 10, 20, 50 };
    public int[] potPrices = { 10, 20, 35, 75 };

    private readonly List<PotShopItem> potItems = new List<PotShopItem>();

    private void OnEnable()
    {
        SpawnPotItems();
        RefreshOwnedCounts();
    }

    private void SpawnPotItems()
    {
        if (potGridParent == null)
        {
            Debug.LogError($"{name}: potGridParent is not assigned");
            return;
        }

        if (potPrefabs == null || potPrefabs.Length == 0)
        {
            Debug.LogError($"{name}: potPrefabs not set");
            return;
        }

        // Clear existing displayed items
        foreach (Transform child in potGridParent)
            Destroy(child.gameObject);

        potItems.Clear();

        int count = Mathf.Min(
            potPrefabs.Length,
            potNames.Length,
            potAmounts.Length,
            potPrices.Length
        );

        for (int i = 0; i < count; i++)
        {
            // Instantiate the PotShopItem component prefab under the grid parent.
            // This overload returns a PotShopItem when the original is a component on the prefab.
            PotShopItem item = Instantiate(potPrefabs[i], potGridParent);

            // Initialize instance with shared dependencies
            item.Initialize(
                potNames[i],
                potAmounts[i],
                potPrices[i],
                growingAreaManager,
                growingPotPrefab
            );

            // Set the current owned count
            var gm = GameManager.Instance;
            int owned = gm != null ? gm.GetPots() : 0;
            item.SetOwned(owned);

            // Subscribe to owned-changed notifications; use += to avoid clobbering other subscribers
            item.OnOwnedChanged += RefreshOwnedCounts;

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

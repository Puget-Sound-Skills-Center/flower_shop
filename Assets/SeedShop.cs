using UnityEngine;

/// <summary>
/// Handles seed purchasing logic.
/// </summary>
public class SeedShop : MonoBehaviour
{
    public FlowerData flower; // Assign the FlowerData in Inspector
    public int seedCost = 2;

    /// <summary>
    /// Attempts to buy a seed by spending money and adding to inventory.
    /// </summary>
    public void BuySeed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing.");
            return;
        }

        if (flower == null)
        {
            Debug.LogWarning("SeedShop: No flower assigned to buy.");
            return;
        }

        if (seedCost <= 0)
        {
            Debug.LogWarning("Seed cost must be greater than zero.");
            return;
        }

        // Attempt to spend money and add seed
        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(flower, 1); // Add the correct seed type
            Debug.Log("Bought 1 " + flower.flowerName + " seed!");
        }
        else
        {
            Debug.Log("Not enough money to buy a seed!");
        }
    }
}

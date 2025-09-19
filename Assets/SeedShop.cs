using UnityEngine;

/// <summary>
/// Handles seed purchasing logic.
/// </summary>
public class SeedShop : MonoBehaviour
{
    public int seedCost = 2;

    /// <summary>
    /// Attempts to buy a seed by spending money and adding to inventory.
    /// </summary>
    public void BuySeed()
    {
        // Null check for GameManager instance
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing.");
            return;
        }

        // Ensure seedCost is positive
        if (seedCost <= 0)
        {
            Debug.LogWarning("Seed cost must be greater than zero.");
            return;
        }

        // Attempt to spend money and add seed
        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(1);
        }
        else
        {
            Debug.Log("Not enough money to buy a seed!");
        }
    }
}

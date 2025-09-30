using UnityEngine;

public class SeedShop : MonoBehaviour
{
    public FlowerData flower; // Assign in Inspector
    public int seedCost = 2;

    public void BuySeed()
    {
        if (GameManager.Instance == null) return;
        if (flower == null) return;

        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(flower, 1);
            Debug.Log("Bought 1 " + flower.flowerName + " seed!");
        }
        else
        {
            Debug.Log("Not enough money to buy a seed!");
        }
    }
}

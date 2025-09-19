using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Money Settings")]
    public int startingMoney = 100;
    public int currentMoney;

    [Header("Seed Inventory")]
    public int seedCount = 0;

    [Header("Flower Inventory")]
    public int flowerCount = 0;

    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;

    // ---- Pot state (single pot only) ----
    public bool potIsGrowing = false;
    public float potEndTime;   // Time.realtimeSinceStartup when growth ends
    public float potGrowTime;  // total grow time

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentMoney = startingMoney;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---- Pot API ----
    public void StartPotGrowth(float growTime)
    {
        potIsGrowing = true;
        potGrowTime = growTime;
        potEndTime = Time.realtimeSinceStartup + growTime;
    }

    public bool GetPotState(out float remaining, out float growTime)
    {
        if (potIsGrowing)
        {
            growTime = potGrowTime;
            remaining = potEndTime - Time.realtimeSinceStartup;

            if (remaining > 0)
                return true;

            // Timer expired → growth ended
            potIsGrowing = false;
        }

        remaining = 0;
        growTime = 0;
        return false;
    }

    // ---- Money / Seeds / Flowers ----
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount) { currentMoney += amount; UpdateMoneyUI(); }
    public void AddSeed(int amount) { seedCount += amount; UpdateSeedUI(); }
    public void AddFlower(int amount) { flowerCount += amount; UpdateFlowerUI(); }

    // ---- UI Updates ----
    public void UpdateAllUI()
    {
        UpdateMoneyUI();
        UpdateSeedUI();
        UpdateFlowerUI();
    }

    private void UpdateMoneyUI() { if (moneyText) moneyText.text = "Money: $" + currentMoney; }
    private void UpdateSeedUI() { if (seedText) seedText.text = "Seeds: " + seedCount; }
    private void UpdateFlowerUI() { if (flowerText) flowerText.text = "Flowers: " + flowerCount; }
}

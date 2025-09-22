using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Decorations")]
    public List<DecorationUnlock> gardenDecorations;

    public int totalFlowersSold = 0; // Track cumulative progress

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
            return;
        }

        // Null checks for UI references
        if (moneyText == null)
            Debug.LogWarning("GameManager: moneyText UI reference is missing.");
        if (seedText == null)
            Debug.LogWarning("GameManager: seedText UI reference is missing.");
        if (flowerText == null)
            Debug.LogWarning("GameManager: flowerText UI reference is missing.");
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

    public void SellFlowers(int sellPricePerFlower, TextMeshProUGUI resultText = null)
    {
        if (flowerCount <= 0)
        {
            if (resultText != null)
                resultText.text = "No flowers to sell!";
            return;
        }

        int earnings = flowerCount * sellPricePerFlower;

        currentMoney += earnings;
        totalFlowersSold += flowerCount; // Track sold progress
        flowerCount = 0; // Reset after selling

        if (resultText != null)
            resultText.text = "Sold flowers for $" + earnings;

        UpdateAllUI();
        CheckForDecorationUnlocks();
    }

    private void CheckForDecorationUnlocks()
    {
        if (gardenDecorations == null) return;
        foreach (var deco in gardenDecorations)
        {
            if (deco != null && !deco.unlocked && totalFlowersSold >= deco.flowersRequired)
            {
                deco.unlocked = true;
                Debug.Log("Unlocked decoration: " + deco.name);
            }
        }
    }

    [System.Serializable]
    public class DecorationUnlock
    {
        public string name;
        public int flowersRequired;
        public GameObject decorationPrefab; // Prefab to spawn when unlocked
        public bool unlocked = false;
    }

    private void UpdateMoneyUI() { if (moneyText != null) moneyText.text = "$" + currentMoney; }
    private void UpdateSeedUI() { if (seedText != null) seedText.text = "Seeds: " + seedCount; }
    private void UpdateFlowerUI() { if (flowerText != null) flowerText.text = "Flowers: " + flowerCount; }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Money Settings")]
    public int startingMoney = 100;
    public int currentMoney;

    [Header("Flower Inventory")]
    public int flowerCount = 0;

    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;
    public Image selectedFlowerIcon; // Shows the currently selected flower

    // ---- Seed Inventory ----
    private Dictionary<FlowerData, int> seedInventory = new Dictionary<FlowerData, int>();

    [Header("Decorations")]
    public List<DecorationUnlock> gardenDecorations;

    public int totalFlowersSold = 0;

    [HideInInspector]
    public FlowerData selectedFlower; // Current selection from seed shop

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentMoney = startingMoney;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ---- Money ----
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

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    // ---- Seeds ----
    public void BuySeed(FlowerData flower)
    {
        if (flower == null) return;

        if (SpendMoney(flower.seedCost))
        {
            if (!seedInventory.ContainsKey(flower))
                seedInventory[flower] = 0;

            seedInventory[flower]++;
        }
        UpdateSeedUI();
    }

    public void AddSeed(FlowerData flower, int amount)
    {
        if (flower == null) return;

        if (!seedInventory.ContainsKey(flower))
            seedInventory[flower] = 0;

        seedInventory[flower] += amount;

        // Ensure it doesn't go below 0
        if (seedInventory[flower] < 0)
            seedInventory[flower] = 0;

        UpdateSeedUI();
    }


    public bool UseSeed(FlowerData flower)
    {
        if (flower == null) return false;

        if (seedInventory.ContainsKey(flower) && seedInventory[flower] > 0)
        {
            seedInventory[flower]--;
            UpdateSeedUI();
            return true;
        }
        return false;
    }

    public int GetSeedCount(FlowerData flower)
    {
        if (flower == null) return 0;
        if (seedInventory.ContainsKey(flower))
            return seedInventory[flower];
        return 0;
    }

    // ---- Flowers ----
    public void AddFlower(int amount)
    {
        flowerCount += amount;
        UpdateFlowerUI();
    }

    // ---- UI Updates ----
    public void UpdateAllUI()
    {
        UpdateMoneyUI();
        UpdateSeedUI();
        UpdateFlowerUI();
        UpdateSelectedFlowerUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + currentMoney;
    }

    private void UpdateSeedUI()
    {
        if (seedText != null)
        {
            // Show total seeds across all types
            int total = 0;
            foreach (var kvp in seedInventory)
                total += kvp.Value;

            seedText.text = "Seeds: " + total;
        }
    }

    private void UpdateFlowerUI()
    {
        if (flowerText != null)
            flowerText.text = "Flowers: " + flowerCount;
    }

    // ---- Selected Flower UI ----
    public void UpdateSelectedFlowerUI()
    {
        if (selectedFlowerIcon != null)
        {
            if (selectedFlower != null && selectedFlower.readySprite != null)
            {
                selectedFlowerIcon.sprite = selectedFlower.readySprite;
                selectedFlowerIcon.enabled = true;
            }
            else
            {
                selectedFlowerIcon.sprite = null;
                selectedFlowerIcon.enabled = false;
            }
        }
    }

    // ---- Selling Flowers ----
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
        totalFlowersSold += flowerCount;
        flowerCount = 0;

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
        public GameObject decorationPrefab;
        public bool unlocked = false;
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Money Settings")]
    public int startingMoney = 100;
    public int currentMoney;

    [Header("Seed Inventory (by flower type)")]
    private Dictionary<FlowerData, int> seedInventory = new Dictionary<FlowerData, int>();

    [Header("Flower Inventory")]
    public int flowerCount = 0;

    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;
    public Image selectedFlowerIcon;

    [HideInInspector] public FlowerData selectedFlower;

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

    // ---- Seeds ----
    public void AddSeed(FlowerData flower, int amount)
    {
        if (flower == null) return;

        if (!seedInventory.ContainsKey(flower))
            seedInventory[flower] = 0;

        seedInventory[flower] += amount;

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
        return seedInventory.ContainsKey(flower) ? seedInventory[flower] : 0;
    }

    // ---- Money / Flowers ----
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
    public void AddFlower(int amount) { flowerCount += amount; UpdateFlowerUI(); }

    // ---- UI Updates ----
    public void UpdateAllUI()
    {
        UpdateMoneyUI();
        UpdateSeedUI();
        UpdateFlowerUI();
        UpdateSelectedFlowerUI();
    }

    private void UpdateMoneyUI() { if (moneyText != null) moneyText.text = "$" + currentMoney; }

    private void UpdateSeedUI()
    {
        if (seedText != null && selectedFlower != null)
        {
            int count = GetSeedCount(selectedFlower);
            seedText.text = $"{selectedFlower.flowerName} Seeds: {count}";
        }
    }

    private void UpdateFlowerUI() { if (flowerText != null) flowerText.text = "Flowers: " + flowerCount; }

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

        UpdateSeedUI(); // refresh seed count when selection changes
    }
}

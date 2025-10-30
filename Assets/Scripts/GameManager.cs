using System;
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

    [Header("Seed Inventory")]
    private Dictionary<FlowerData, int> seedInventory = new Dictionary<FlowerData, int>();

    [Header("Flower Inventory")]
    private Dictionary<FlowerData, int> flowerInventory = new Dictionary<FlowerData, int>();


    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;
    public Image selectedFlowerIcon;

    [HideInInspector]
    public FlowerData selectedFlower;

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
        UpdateAllUI();
    }

    // ---- Inventory Methods ----
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

    public void AddSeed(FlowerData flower, int amount)
    {
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

    public FlowerData GetPlantableFlower(FlowerData requestedFlower)
    {
        if (requestedFlower != null && GetSeedCount(requestedFlower) > 0)
            return requestedFlower;
        foreach (var kvp in seedInventory)
        {
            if (kvp.Value > 0)
                return kvp.Key;
        }
        return null;
    }

    public void AddFlower(FlowerData flower, int amount)
    {
        if (flower == null)
        {
            Debug.LogWarning("Tried to add a flower with null FlowerData.");
            return;
        }

        if (!flowerInventory.ContainsKey(flower))
            flowerInventory[flower] = 0;

        flowerInventory[flower] += amount;
        if (flowerInventory[flower] < 0)
            flowerInventory[flower] = 0;

        UpdateFlowerUI(); // UI refresh for totals
    }


    public int GetFlowerCount(FlowerData flower)
    {
        if (flower == null) return 0;
        return flowerInventory.ContainsKey(flower) ? flowerInventory[flower] : 0;
    }


    private void UpdateMoneyUI() { if (moneyText != null) moneyText.text = "$" + currentMoney; }

    private void UpdateSeedUI()
    {
        if (seedText != null)
        {
            if (selectedFlower != null)
                seedText.text = "Seeds: " + GetSeedCount(selectedFlower);
            else
                seedText.text = "Seeds: 0";
        }
    }

    private Dictionary<FlowerData, int> bouquetInventory = new Dictionary<FlowerData, int>();

    public void AddBouquet(FlowerData flower)
    {
        if (flower == null)
        {
            Debug.LogWarning("GameManager: Tried to add a bouquet with null FlowerData.");
            return;
        }

        if (!bouquetInventory.ContainsKey(flower))
            bouquetInventory[flower] = 0;

        bouquetInventory[flower]++;

        Debug.Log($"Added 1 {flower.name} bouquet! Total: {bouquetInventory[flower]}");
    }

    public Dictionary<FlowerData, int> GetBouquetInventory()
    {
        return bouquetInventory;
    }

    public Dictionary<FlowerData, int> GetFlowerInventory()
    {
        return flowerInventory;
    }




    private void UpdateFlowerUI()
    {
        if (flowerText == null) return;

        int total = 0;
        foreach (var kvp in flowerInventory)
            total += kvp.Value;

        flowerText.text = "Flowers: " + total;
    }


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
        UpdateSeedUI();
    }

    public void UpdateAllUI()
    {
        UpdateMoneyUI();
        UpdateSeedUI();
        UpdateFlowerUI();
        UpdateSelectedFlowerUI();
    }

}

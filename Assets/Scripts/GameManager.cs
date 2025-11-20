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

    [Header("Selected Flower")]
    public FlowerData SelectedFlowerData;   // <-- USED BY POTS!

    [Header("Seed Inventory")]
    private Dictionary<FlowerData, int> seedInventory = new Dictionary<FlowerData, int>();

    [Header("Harvested Flower Inventory")]
    private Dictionary<FlowerData, int> flowerInventory = new Dictionary<FlowerData, int>();

    [Header("Bouquet Inventory")]
    private Dictionary<FlowerData, int> bouquetInventory = new Dictionary<FlowerData, int>();

    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;
    public Image selectedFlowerIcon;

    public List<FlowerData> seedTypes = new List<FlowerData>();
    public int selectedSeedIndex = 0;




    // ------------------------------------------
    // Singleton Setup
    // ------------------------------------------
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


    // ------------------------------------------
    // Money
    // ------------------------------------------
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


    // ------------------------------------------
    // Seeds
    // ------------------------------------------
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
            // Automatically switch to next seed if seeds run out
            if (seedInventory[flower] == 0 && SelectedFlowerData == flower)
            {
                AutoSelectNextAvailableSeed();
            }

            UpdateSeedUI();
            return true;
        }

        return false;
    }

    public void AutoSelectNextAvailableSeed()
    {
        if (seedTypes.Count == 0)
            return;

        int startIndex = selectedSeedIndex;

        // Try all seeds until we find one with inventory
        for (int i = 0; i < seedTypes.Count; i++)
        {
            int index = (startIndex + 1 + i) % seedTypes.Count;
            FlowerData next = seedTypes[index];

            if (GetSeedCount(next) > 0)
            {
                selectedSeedIndex = index;
                SelectedFlowerData = next;
                UpdateSelectedFlowerUI();
                return;
            }
        }

        // No seeds left at all
        SelectedFlowerData = null;
        UpdateSelectedFlowerUI();
    }

    private void Update()
    {
        HandleSeedHotkeys();
    }

    private void HandleSeedHotkeys()
    {
        // supports 1–9 hotkeys
        for (int i = 0; i < seedTypes.Count && i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                SelectSeedByIndex(i);
            }
        }
    }

    public void SelectSeedByIndex(int index)
    {
        if (index < 0 || index >= seedTypes.Count)
            return;

        FlowerData flower = seedTypes[index];

        // Only select if we have seeds, otherwise ignore
        if (GetSeedCount(flower) > 0)
        {
            selectedSeedIndex = index;
            SelectedFlowerData = flower;
            UpdateSelectedFlowerUI();
        }
    }



    public int GetSeedCount(FlowerData flower)
    {
        if (flower == null) return 0;

        return seedInventory.ContainsKey(flower)
            ? seedInventory[flower]
            : 0;
    }


    // ------------------------------------------
    // Harvested Flowers
    // ------------------------------------------
    public void AddFlower(FlowerData flower, int amount)
    {
        if (flower == null)
        {
            Debug.LogWarning("Tried to add flower with null FlowerData.");
            return;
        }

        if (!flowerInventory.ContainsKey(flower))
            flowerInventory[flower] = 0;

        flowerInventory[flower] += amount;
        if (flowerInventory[flower] < 0)
            flowerInventory[flower] = 0;

        UpdateFlowerUI();
    }

    public int GetFlowerCount(FlowerData flower)
    {
        if (flower == null) return 0;

        return flowerInventory.ContainsKey(flower)
            ? flowerInventory[flower]
            : 0;
    }


    public Dictionary<FlowerData, int> GetFlowerInventory()
    {
        return flowerInventory;
    }


    // ------------------------------------------
    // Bouquets
    // ------------------------------------------
    public void AddBouquet(FlowerData flower)
    {
        if (flower == null)
        {
            Debug.LogWarning("Tried to add a bouquet with null FlowerData.");
            return;
        }

        if (!bouquetInventory.ContainsKey(flower))
            bouquetInventory[flower] = 0;

        bouquetInventory[flower]++;

        Debug.Log($"Added bouquet for {flower.name}. Total: {bouquetInventory[flower]}");
    }

    public Dictionary<FlowerData, int> GetBouquetInventory()
    {
        return bouquetInventory;
    }


    // ------------------------------------------
    // UI
    // ------------------------------------------
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + currentMoney;
    }

    private void UpdateSeedUI()
    {
        if (seedText != null)
        {
            if (SelectedFlowerData != null)
                seedText.text = "Seeds: " + GetSeedCount(SelectedFlowerData);
            else
                seedText.text = "Seeds: 0";
        }
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
            if (SelectedFlowerData != null && SelectedFlowerData.readySprite != null)
            {
                selectedFlowerIcon.sprite = SelectedFlowerData.readySprite;
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


    // ------------------------------------------
    // Pots
    // ------------------------------------------
    private int potCount = 0;

    public void AddPots(int amount)
    {
        potCount += amount;
        if (potCount < 0) potCount = 0;
    }

    public int GetPots()
    {
        return potCount;
    }
}

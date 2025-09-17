using UnityEngine;
using TMPro;

/// <summary>
/// Manages game state, including money, seed inventory, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Money Settings")]
    public int startingMoney = 100;
    public int currentMoney;

    [Header("Seed Inventory")]
    public int seedCount = 0;
    public TextMeshProUGUI seedText;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Uncomment if you want persistence across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
        UpdateSeedUI();
    }

    /// <summary>
    /// Attempts to spend the specified amount of money.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    /// <summary>
    /// Adds the specified amount of money.
    /// </summary>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    /// <summary>
    /// Adds the specified amount of seeds.
    /// </summary>
    public void AddSeed(int amount)
    {
        seedCount += amount;
        UpdateSeedUI();
    }

    /// <summary>
    /// Attempts to spend the specified amount of seeds.
    /// </summary>
    public bool SpendSeed(int amount)
    {
        if (seedCount >= amount)
        {
            seedCount -= amount;
            UpdateSeedUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough seeds!");
            return false;
        }
    }

    /// <summary>
    /// Updates the money display UI.
    /// </summary>
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Money: ${currentMoney}";
        else
            Debug.LogWarning("MoneyText UI reference is missing.");
    }

    /// <summary>
    /// Updates the seed display UI.
    /// </summary>
    private void UpdateSeedUI()
    {
        if (seedText != null)
            seedText.text = $"Seeds: {seedCount}";
        else
            Debug.LogWarning("SeedText UI reference is missing.");
    }
}
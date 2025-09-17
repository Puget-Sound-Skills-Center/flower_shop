using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ keep between scenes
            SceneManager.sceneLoaded += OnSceneLoaded; // listen for scene changes
        }
        else
        {
            Destroy(gameObject); // ✅ prevent duplicates
        }
    }

    private void Start()
    {
        currentMoney = startingMoney;
        UpdateAllUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to re-hook UI automatically when entering a new scene
        UIInitializer uiInit = FindObjectOfType<UIInitializer>();
        if (uiInit != null)
        {
            moneyText = uiInit.moneyText;
            seedText = uiInit.seedText;
            flowerText = uiInit.flowerText;
            UpdateAllUI();
        }
    }

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

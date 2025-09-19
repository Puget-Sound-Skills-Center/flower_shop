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

    [Header("UI (auto-linked by scene)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;

    // ---- Pot state (single pot only) ----
    public bool potIsGrowing = false;
    public float potEndTime = 0f;   // Time.realtimeSinceStartup when growth ends
    public float potGrowTime = 0f;  // total grow time

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            currentMoney = startingMoney;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Auto-find UI by name in the new scene
        moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
        seedText = GameObject.Find("SeedText")?.GetComponent<TextMeshProUGUI>();
        flowerText = GameObject.Find("FlowerText")?.GetComponent<TextMeshProUGUI>();

        UpdateAllUI();
    }

    private void Update()
    {
        // If pot was growing and finished while we were away, complete it
        if (potIsGrowing && Time.realtimeSinceStartup >= potEndTime)
        {
            potIsGrowing = false;
            AddFlower(1);
            Debug.Log("[GameManager] Pot finished while away, flower added!");
        }
    }

    // ---- Pot API ----
    public void StartPotGrowth(float growTime)
    {
        potIsGrowing = true;
        potGrowTime = growTime;
        potEndTime = Time.realtimeSinceStartup + growTime;
    }

    public void SetPotGrowth(float growTime)
    {
        potIsGrowing = true;
        potGrowTime = growTime;
        potEndTime = Time.realtimeSinceStartup + growTime;
    }

    public void ClearPotGrowth()
    {
        potIsGrowing = false;
        potEndTime = 0f;
        potGrowTime = 0f;
    }

    public bool GetPotState(out float remaining, out float growTime)
    {
        if (potIsGrowing)
        {
            growTime = potGrowTime;
            remaining = potEndTime - Time.realtimeSinceStartup;
            return remaining > 0;
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

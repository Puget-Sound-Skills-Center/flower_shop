using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PotState
{
    public string potID;
    public bool isGrowing;
    public float endTime;   // Time.realtimeSinceStartup when growth finishes
    public float growTime;  // total grow time used for progress calculations

    public PotState(string id, float end, float grow)
    {
        potID = id;
        isGrowing = true;
        endTime = end;
        growTime = grow;
    }
}

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

    // Pot states stored by potID
    private Dictionary<string, PotState> potStates = new Dictionary<string, PotState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // ❌ Remove this line, otherwise money resets if scene reloads
            // currentMoney = startingMoney;
        }

    }

    private void Start()
    {
        if (currentMoney == 0) currentMoney = startingMoney;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
        seedText = GameObject.Find("SeedText")?.GetComponent<TextMeshProUGUI>();
        flowerText = GameObject.Find("FlowerText")?.GetComponent<TextMeshProUGUI>();

        Invoke(nameof(UpdateAllUI), 0.05f); // small delay to let UI initialize
    }


    private void Update()
    {
        // Check for pot states that finished while player was in other scenes
        float now = Time.realtimeSinceStartup;
        List<string> finished = null;

        foreach (var kvp in potStates)
        {
            var ps = kvp.Value;
            if (ps.isGrowing && now >= ps.endTime)
            {
                if (finished == null) finished = new List<string>();
                finished.Add(kvp.Key);
            }
        }

        if (finished != null)
        {
            foreach (var id in finished)
            {
                // TryCompletePot is idempotent: it will only add a flower once and remove the state.
                TryCompletePot(id);
            }
        }
    }

    // ----------------------
    // Pot state API
    // ----------------------

    // Start or overwrite growth for a pot (called when player plants)
    public void StartPotGrowth(string potID, float growTime)
    {
        float end = Time.realtimeSinceStartup + growTime;
        if (potStates.ContainsKey(potID))
        {
            potStates[potID].isGrowing = true;
            potStates[potID].endTime = end;
            potStates[potID].growTime = growTime;
        }
        else
        {
            potStates[potID] = new PotState(potID, end, growTime);
        }
    }

    // Try to read a pot state. Returns true if a state exists.
    public bool TryGetPotState(string potID, out PotState state)
    {
        return potStates.TryGetValue(potID, out state);
    }

    // Attempt to complete a pot (idempotent). Returns true if a completion was processed now.
    public bool TryCompletePot(string potID)
    {
        if (potStates.TryGetValue(potID, out PotState ps))
        {
            float now = Time.realtimeSinceStartup;
            if (now >= ps.endTime)
            {
                AddFlower(1);           // award flower
                potStates.Remove(potID); // clear pot state
                Debug.Log($"[GameManager] Pot '{potID}' completed while away. Flower added.");
                return true;
            }
        }
        return false;
    }

    // Force clear (if you need it)
    public void ClearPotState(string potID)
    {
        if (potStates.ContainsKey(potID)) potStates.Remove(potID);
    }

    // ----------------------
    // Money / resources API
    // ----------------------
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        Debug.Log("Not enough money!");
        return false;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public void AddSeed(int amount)
    {
        seedCount += amount;
        UpdateSeedUI();
    }

    public void AddFlower(int amount)
    {
        flowerCount += amount;
        UpdateFlowerUI();
    }

    // ----------------------
    // UI updates
    // ----------------------
    public void UpdateAllUI()
    {
        UpdateMoneyUI();
        UpdateSeedUI();
        UpdateFlowerUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText)
            moneyText.text = "$" + currentMoney;
        else
            Debug.LogWarning("MoneyText not found in scene!");
    }

    private void UpdateSeedUI() { if (seedText) seedText.text = "Seeds: " + seedCount; }
    private void UpdateFlowerUI() { if (flowerText) flowerText.text = "Flowers: " + flowerCount; }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotShopItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text ownedText;
    public Button buyButton;

    [Header("Growing Area")]
    public GrowingAreaManager growingAreaManager; // Reference to the GrowingAreaManager
    public GameObject growingPotPrefab;           // Prefab to spawn in the backroom

    [HideInInspector] public string potDisplayName;
    [HideInInspector] public int potsOnSale;
    [HideInInspector] public int price;

    private int owned;

    public System.Action OnOwnedChanged;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuyPot);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (nameText != null)
            nameText.text = $"{potDisplayName} ({potsOnSale} pots)";

        if (costText != null)
            costText.text = $"${price}";

        if (ownedText != null)
            ownedText.text = $"Owned: {owned}";
    }

    public void SetOwned(int ownedCount)
    {
        owned = ownedCount;
        if (ownedText != null)
            ownedText.text = $"Owned: {owned}";
    }

    public void BuyPot()
    {
        if (growingAreaManager == null || growingPotPrefab == null)
        {
            Debug.LogWarning("PotShopItem: GrowingAreaManager or growingPotPrefab not assigned.");
            return;
        }

        // Check available space
        int availableSpace = growingAreaManager.maxPots - growingAreaManager.GetCurrentPotCount();
        if (availableSpace <= 0)
        {
            Debug.Log("Cannot buy more pots. Growing area is full.");
            return;
        }

        // Determine how many pots we can actually spawn
        int potsToSpawn = Mathf.Min(potsOnSale, availableSpace);

        // Check if player has enough money
        var gm = GameManager.Instance;
        if (gm == null || !gm.SpendMoney(price))
        {
            Debug.Log("Not enough money to buy pot bundle.");
            return;
        }

        // Add pots to inventory
        gm.AddPots(potsToSpawn);
        owned += potsToSpawn;
        UpdateUI();

        // Spawn pots in the growing area
        growingAreaManager.AddPots(potsToSpawn, growingPotPrefab);

        Debug.Log($"Bought {potsToSpawn} pots ({potDisplayName}) for ${price}. Total owned: {owned}");

        OnOwnedChanged?.Invoke();
    }
}

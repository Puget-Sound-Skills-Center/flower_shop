using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotShopItem : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text ownedText;
    public Button buyButton;

    private GrowingAreaManager growingAreaManager;
    private GameObject growingPotPrefab;

    private string potDisplayName;
    private int potsOnSale;
    private int price;
    private int owned;

    public System.Action OnOwnedChanged;

    private void Awake()
    {
        // Do not RemoveAllListeners here; initialization hooks are added in Initialize.
        if (buyButton == null)
        {
            // It's normal for this to be assigned in the prefab, but log if missing.
            Debug.LogWarning($"{name}: buyButton is not assigned on Awake");
        }
    }

    public void Initialize(
        string displayName,
        int amount,
        int cost,
        GrowingAreaManager areaManager,
        GameObject potPrefab)
    {
        potDisplayName = displayName;
        potsOnSale = amount;
        price = cost;
        growingAreaManager = areaManager;
        growingPotPrefab = potPrefab;

        if (growingAreaManager == null)
            Debug.LogError($"{name}: GrowingAreaManager missing");

        if (growingPotPrefab == null)
            Debug.LogError($"{name}: GrowingPotPrefab missing");

        // Hook the buy button safely here (after the instance is created and fields set)
        if (buyButton != null)
        {
            // Remove only this listener to avoid duplicates, preserve other configured listeners.
            buyButton.onClick.RemoveListener(BuyPot);
            buyButton.onClick.AddListener(BuyPot);
        }
        else
        {
            Debug.LogError($"{name}: buyButton is not assigned");
        }

        UpdateUI();
    }

    public void SetOwned(int count)
    {
        owned = count;
        if (ownedText != null)
            ownedText.text = $"Owned: {owned}";
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

    public void BuyPot()
    {
        Debug.Log("BuyPot CLICKED");

        if (growingAreaManager == null || growingPotPrefab == null)
        {
            Debug.LogError("PotShopItem not initialized");
            return;
        }

        int available = growingAreaManager.maxPots - growingAreaManager.GetCurrentPotCount();

        if (available <= 0)
        {
            Debug.Log("Growing area full");
            return;
        }

        int toSpawn = Mathf.Min(potsOnSale, available);

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager instance missing");
            return;
        }

        // Spend the bundle price (keeps original behavior). If you want per-pot pricing,
        // change to gm.SpendMoney(price * toSpawn) and adjust UI accordingly.
        if (!gm.SpendMoney(price))
        {
            Debug.Log("Not enough money");
            return;
        }

        gm.AddPots(toSpawn);
        owned += toSpawn;
        UpdateUI();

        growingAreaManager.AddPots(toSpawn, growingPotPrefab);

        Debug.Log($"Bought {toSpawn} pots for ${price}");
        OnOwnedChanged?.Invoke();
    }

    private void OnDestroy()
    {
        // Remove the listener to avoid dangling references
        if (buyButton != null)
            buyButton.onClick.RemoveListener(BuyPot);
    }
}

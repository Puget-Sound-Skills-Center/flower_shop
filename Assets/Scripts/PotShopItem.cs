using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotShopItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;   // Shows the pot bundle name
    public TMP_Text costText;   // Shows the cost ($)
    public TMP_Text ownedText;  // Shows how many pots player owns
    public Button buyButton;

    // Bundle data is fully assigned by PotsTab.cs
    [HideInInspector] public string potName;
    [HideInInspector] public int bundleAmount;
    [HideInInspector] public int cost;
    [HideInInspector] public int owned;

    // Callback to notify the parent tab when a purchase occurs
    public System.Action OnPotPurchased;

    private void Awake()
    {
        // Always ensure buyButton is wired up
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuyPot);
        }
        else
        {
            Debug.LogWarning("PotShopItem: buyButton not assigned.");
        }
    }

    /// <summary>
    /// Update the UI with current bundle info
    /// </summary>
    public void UpdateUI()
    {
        if (nameText != null)
            nameText.text = $"{potName} ({bundleAmount})";

        if (costText != null)
            costText.text = $"${cost}";

        if (ownedText != null)
            ownedText.text = $"Owned: {owned}";
    }

    /// <summary>
    /// Set the number of pots owned (used by PotsTab to refresh UI dynamically)
    /// </summary>
    public void SetOwned(int ownedCount)
    {
        owned = ownedCount;
        if (ownedText != null)
            ownedText.text = $"Owned: {owned}";
    }

    /// <summary>
    /// Handles buying this pot bundle
    /// </summary>
    private void BuyPot()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogWarning("PotShopItem: GameManager not found!");
            return;
        }

        // Defensive: cost and bundleAmount should be positive
        if (cost <= 0 || bundleAmount <= 0)
        {
            Debug.LogWarning($"PotShopItem: Invalid cost ({cost}) or bundleAmount ({bundleAmount}) for {potName}.");
            return;
        }

        if (gm.SpendMoney(cost))
        {
            gm.AddPots(bundleAmount);
            owned += bundleAmount;
            UpdateUI();

            Debug.Log($"Bought {bundleAmount} pots ({potName}) for ${cost}. Total owned: {owned}");

            OnPotPurchased?.Invoke();
        }
        else
        {
            Debug.Log("Not enough money to buy pot bundle.");
        }
    }
}

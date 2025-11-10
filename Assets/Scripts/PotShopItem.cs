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

    [HideInInspector] public string potDisplayName;
    [HideInInspector] public int potsOnSale;
    [HideInInspector] public int price;
    private int owned;

    public System.Action OnOwnedChanged;

    private void Start()
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

    private void BuyPot()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogWarning("PotShopItem: GameManager not found!");
            return;
        }

        if (gm.SpendMoney(price))
        {
            gm.AddPots(potsOnSale);
            owned += potsOnSale;
            UpdateUI();

            Debug.Log($"Bought {potsOnSale} pots for ${price}. Total owned: {owned}");
            OnOwnedChanged?.Invoke();
        }
        else
        {
            Debug.Log("Not enough money to buy pot bundle.");
        }
    }
}

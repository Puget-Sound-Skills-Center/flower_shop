using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotShopItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI potNameText;
    public TextMeshProUGUI potCostText;
    public Button buyButton;

    [Header("Pot Settings")]
    public int potCount = 1;
    public int potCost = 10;

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyPot);
    }

    public void SetupItem(int count, int cost)
    {
        potCount = count;
        potCost = cost;

        if (potNameText != null)
            potNameText.text = $"{count} Pot{(count > 1 ? "s" : "")}";

        if (potCostText != null)
            potCostText.text = $"${cost}";
    }

    private void OnBuyPot()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.money >= potCost)
        {
            gm.AddMoney(-potCost);
            gm.AddPots(potCount);
            Debug.Log($"Bought {potCount} pot(s) for ${potCost}");
        }
        else
        {
            Debug.Log("Not enough money to buy pots!");
        }
    }
}

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

    [Header("Backroom Parent")]
    public GameObject growingAreaParent; // Assign the parent GameObject for spawned pots
    public GameObject growingPotPrefab;  // Assign the actual Pot prefab to spawn

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

    private void BuyPot()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogWarning("PotShopItem: GameManager not found!");
            return;
        }

        if (!gm.SpendMoney(price))
        {
            Debug.Log("Not enough money to buy pot bundle.");
            return;
        }

        // Add pots to inventory
        gm.AddPots(potsOnSale);
        owned += potsOnSale;
        UpdateUI();

        // Spawn the pots in the backroom using just a parent GameObject
        if (growingAreaParent != null && growingPotPrefab != null)
        {
            for (int i = 0; i < potsOnSale; i++)
            {
                GameObject newPot = Instantiate(growingPotPrefab, growingAreaParent.transform);
                // Optional: offset each pot slightly so they don’t overlap
                newPot.transform.localPosition = new Vector3(i * 1.5f, 0, 0);
                newPot.transform.localRotation = Quaternion.identity;
                newPot.transform.localScale = Vector3.one;
            }
        }

        Debug.Log($"Bought {potsOnSale} pots ({potDisplayName}) for ${price}. Total owned: {owned}");

        // Notify parent UI to refresh if needed
        OnOwnedChanged?.Invoke();
    }
}

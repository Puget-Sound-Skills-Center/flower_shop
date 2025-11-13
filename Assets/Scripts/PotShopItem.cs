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

    [Header("Backroom Assignment")]
    [Tooltip("Assign either the GrowingAreaManager script or its parent GameObject.")]
    public GrowingAreaManager growingAreaManager; // Preferred: assign script directly
    public GameObject growingAreaParent;          // Alternative: assign parent GameObject

    [Header("Pot Prefab")]
    public GameObject growingPotPrefab; // Assign the actual Pot prefab to spawn

    [HideInInspector] public string potDisplayName;
    [HideInInspector] public int potsOnSale;
    [HideInInspector] public int price;
    private int owned;

    public System.Action OnOwnedChanged;

    private void Awake()
    {
        // If only the parent is assigned, get the GrowingAreaManager from it
        if (growingAreaManager == null && growingAreaParent != null)
        {
            growingAreaManager = growingAreaParent.GetComponent<GrowingAreaManager>();
            if (growingAreaManager == null)
                Debug.LogWarning("PotShopItem: growingAreaParent assigned but no GrowingAreaManager found on it.");
        }

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

        // Update inventory
        gm.AddPots(potsOnSale);
        owned += potsOnSale;
        UpdateUI();

        // Spawn pots in the growing area using GrowingAreaManager
        if (growingAreaManager != null)
        {
            growingAreaManager.AddPots(potsOnSale);
        }
        else
        {
            Debug.LogWarning("PotShopItem: No GrowingAreaManager assigned or found on parent.");
        }

        Debug.Log($"Bought {potsOnSale} pots ({potDisplayName}) for ${price}. Total owned: {owned}");

        OnOwnedChanged?.Invoke();
    }
}

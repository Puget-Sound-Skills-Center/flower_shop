using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Shop : MonoBehaviour
{
    public Button sellButton;
    public TextMeshProUGUI sellResultText;
    public int sellPricePerFlower = 5; // Adjust price per flower

    [Header("Bouquet Shelf UI")]
    public Transform shelfArea; // Assign in Inspector
    public GameObject bouquetDisplayPrefab; // Assign in Inspector

    private void Start()
    {
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellFlowers);
        else
            Debug.LogWarning("Shop: sellButton reference is missing.");

        if (sellResultText == null)
            Debug.LogWarning("Shop: sellResultText reference is missing.");
    }

    private void OnSellFlowers()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Shop: GameManager instance is missing.");
            return;
        }

        int prevFlowerCount = GameManager.Instance.flowerCount;

        // Sell all flowers for money
        if (prevFlowerCount > 0)
        {
            int totalEarned = prevFlowerCount * sellPricePerFlower;
            GameManager.Instance.AddMoney(totalEarned);
            GameManager.Instance.AddFlower(-prevFlowerCount);

            if (sellResultText != null)
                sellResultText.text = $"Sold {prevFlowerCount} flowers for ${totalEarned}!";
        }
        else
        {
            if (sellResultText != null)
                sellResultText.text = "No flowers to sell!";
            StartCoroutine(ResetResultText());
        }
    }

    public void LoadBouquetsOnShelf()
    {
        if (shelfArea == null || bouquetDisplayPrefab == null)
        {
            Debug.LogWarning("Shop: shelfArea or bouquetDisplayPrefab not assigned.");
            return;
        }

        foreach (Transform child in shelfArea)
            Destroy(child.gameObject);

        foreach (var kvp in GameManager.Instance.GetBouquetInventory())
        {
            GameObject bouquet = Instantiate(bouquetDisplayPrefab, shelfArea);
            var img = bouquet.GetComponent<Image>();
            if (img != null)
                img.sprite = kvp.Key.readySprite;
        }
    }

    private IEnumerator ResetResultText()
    {
        yield return new WaitForSeconds(1f);
        if (sellResultText != null)
            sellResultText.text = "";
    }
}

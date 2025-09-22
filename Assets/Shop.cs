using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // Needed for IEnumerator

public class Shop : MonoBehaviour
{
    public Button sellButton;
    public TextMeshProUGUI sellResultText;
    public int sellPricePerFlower = 5; // Adjust price per flower

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
        GameManager.Instance.SellFlowers(sellPricePerFlower, sellResultText);

        // If no flowers, start coroutine to reset text
        if (prevFlowerCount <= 0 && sellResultText != null)
            StartCoroutine(ResetResultText());
    }

    private IEnumerator ResetResultText()
    {
        yield return new WaitForSeconds(1f);
        sellResultText.text = "";
    }
}

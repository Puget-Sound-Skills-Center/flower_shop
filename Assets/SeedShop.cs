using UnityEngine;
using TMPro;
using System.Collections;

public class SeedShop : MonoBehaviour
{
    [Header("Seed Settings")]
    public int seedCost = 2;

    [Header("Flower Sell Settings")]
    public int sellPrice = 5; // How much one flower sells for

    [Header("Player Feedback")]
    public TextMeshProUGUI sellFeedbackText; // Assign in Inspector
    public float feedbackDuration = 2f;      // Seconds feedback stays visible

    private Coroutine feedbackCoroutine;

    public void BuySeed(FlowerData flower)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing.");
            return;
        }

        if (flower == null)
        {
            Debug.LogError("No flower assigned for purchase.");
            return;
        }

        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(flower, 1);
            ShowFeedback("Bought 1 " + flower.name + " seed!");
        }
        else
        {
            ShowFeedback("Not enough money to buy a seed!");
        }
    }


    public void SellFlower()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing.");
            return;
        }

        // Check if player has flowers to sell
        if (GameManager.Instance.flowerCount > 0)
        {
            GameManager.Instance.AddFlower(-1); // remove 1 flower
            GameManager.Instance.AddMoney(sellPrice); // add money
            ShowFeedback("Sold  for $" + sellPrice + "!");
        }
        else
        {
            ShowFeedback("No flowers to sell!");
        }
    }

    private void ShowFeedback(string message)
    {
        if (sellFeedbackText == null) return;

        // Stop any existing coroutine
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        sellFeedbackText.text = message;
        sellFeedbackText.gameObject.SetActive(true);

        feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (sellFeedbackText != null)
            sellFeedbackText.gameObject.SetActive(false);
    }
}

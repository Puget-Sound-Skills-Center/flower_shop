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

    // Reference to the confirmation window prefab (assign in Inspector)
    public GameObject confirmationWindowPrefab;

    private GameObject currentConfirmation;

    /// <summary>
    /// Called when player clicks "Buy" on a seed packet
    /// </summary>
    public void ShowBuyConfirmation(FlowerData flower, Transform spawnPoint)
    {
        if (confirmationWindowPrefab == null)
        {
            Debug.LogError("Confirmation prefab not assigned!");
            return;
        }

        // Destroy any existing window
        if (currentConfirmation != null)
            Destroy(currentConfirmation);

        // Spawn next to the packet
        currentConfirmation = Instantiate(confirmationWindowPrefab, spawnPoint.position, Quaternion.identity);

        // Make sure it’s on canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
            currentConfirmation.transform.SetParent(canvas.transform, false);

        // Set up text
        TextMeshProUGUI confirmText = currentConfirmation.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmText != null)
            confirmText.text = $"Buy 1 {flower.name} seed for ${seedCost}?";

        // Hook up buttons
        ConfirmationWindow window = currentConfirmation.GetComponent<ConfirmationWindow>();
        if (window != null)
        {
            window.Setup(() => { ConfirmBuy(flower); }, CancelBuy);
        }
    }

    private void ConfirmBuy(FlowerData flower)
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(flower, 1);
            ShowFeedback("Bought 1 " + flower.name + " seed!");
        }
        else
        {
            ShowFeedback("Not enough money!");
        }

        CancelBuy(); // close popup
    }

    private void CancelBuy()
    {
        if (currentConfirmation != null)
        {
            Destroy(currentConfirmation);
            currentConfirmation = null;
        }
    }

    // ---- Sell Flower ----
    public void SellFlower()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.flowerCount > 0)
        {
            GameManager.Instance.AddFlower(-1);
            GameManager.Instance.AddMoney(sellPrice);
            ShowFeedback("Sold for $" + sellPrice + "!");
        }
        else
        {
            ShowFeedback("No flowers to sell!");
        }
    }

    // ---- Feedback UI ----
    private void ShowFeedback(string message)
    {
        if (sellFeedbackText == null) return;

        // If the dissolve component exists, use it
        TextWaterfallDissolve dissolve = sellFeedbackText.GetComponent<TextWaterfallDissolve>();
        if (dissolve != null)
        {
            dissolve.PlayDissolve(message);
        }
        else
        {
            // Fallback to old static display if dissolve missing
            sellFeedbackText.text = message;
            sellFeedbackText.gameObject.SetActive(true);

            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
        }
    }


    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (sellFeedbackText != null)
            sellFeedbackText.gameObject.SetActive(false);
    }
}

using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class SeedShop : MonoBehaviour
{
    [Header("Seed Settings")]
    public int seedCost = 2;

    [Header("Flower Sell Settings")]
    public int sellPrice = 5;

    [Header("Player Feedback")]
    public TextMeshProUGUI sellFeedbackText; // Text near Sell button
    public TextMeshProUGUI buyFeedbackText;  // New text near Seed area
    public float feedbackDuration = 2f;

    private Coroutine sellFeedbackRoutine;
    private Coroutine buyFeedbackRoutine;

    [Header("Confirmation Window")]
    public GameObject confirmationWindowPrefab;
    private GameObject currentConfirmation;

    [Header("UI Canvas")]
    public Canvas targetCanvas; // Assign your main UI canvas in the Inspector (falls back to FindObjectOfType)

    // -------- BUY SEED --------
    public void ShowBuyConfirmation(FlowerData flower, Transform spawnPoint)
    {
        if (confirmationWindowPrefab == null)
        {
            Debug.LogError("Confirmation prefab not assigned!");
            return;
        }

        if (currentConfirmation != null)
            Destroy(currentConfirmation);

        // Find canvas (prefer assigned targetCanvas)
        Canvas canvas = targetCanvas != null ? targetCanvas : FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("SeedShop: No Canvas found in scene. Instantiating confirmation in world space.");
            currentConfirmation = Instantiate(confirmationWindowPrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
        }
        else
        {
            currentConfirmation = Instantiate(confirmationWindowPrefab);
            currentConfirmation.transform.SetParent(canvas.transform, false);

            // Center the popup in the canvas
            RectTransform popupRect = currentConfirmation.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                popupRect.anchorMin = new Vector2(0.5f, 0.5f);
                popupRect.anchorMax = new Vector2(0.5f, 0.5f);
                popupRect.pivot = new Vector2(0.5f, 0.5f);
                popupRect.anchoredPosition = Vector2.zero;
            }
        }

        TextMeshProUGUI confirmText = currentConfirmation.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmText != null)
            confirmText.text = $"Buy 1 {flower.name} seed for ${seedCost}?";

        ConfirmationWindow window = currentConfirmation.GetComponent<ConfirmationWindow>();
        if (window != null)
        {
            // Use explicit Action delegates to avoid delegate-typing/overload issues.
            Action onConfirm = new Action(() => ConfirmBuy(flower));
            Action onCancel = new Action(CancelBuy);
            window.Setup(onConfirm, onCancel);
        }
        else
        {
            Debug.LogError("SeedShop: confirmationWindowPrefab does not contain a ConfirmationWindow component.");
        }
    }

    private void ConfirmBuy(FlowerData flower)
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.SpendMoney(seedCost))
        {
            GameManager.Instance.AddSeed(flower, 1);
            ShowBuyFeedback($"Bought 1 {flower.name} seed!");
        }
        else
        {
            ShowBuyFeedback("Not enough money!");
        }

        CancelBuy();
    }

    private void CancelBuy()
    {
        if (currentConfirmation != null)
        {
            Destroy(currentConfirmation);
            currentConfirmation = null;
        }
    }

    // -------- SELL FLOWER --------
    public void SellFlower()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.flowerCount > 0)
        {
            GameManager.Instance.AddFlower(-1);
            GameManager.Instance.AddMoney(sellPrice);
            ShowSellFeedback($"Sold for ${sellPrice}!");
        }
        else
        {
            ShowSellFeedback("No flowers to sell!");
        }
    }

    // -------- FEEDBACK HANDLERS --------
    private void ShowBuyFeedback(string message)
    {
        if (buyFeedbackText == null)
        {
            Debug.LogWarning("Buy feedback text not assigned!");
            return;
        }

        // Prevent overlap: disable sell feedback if using the same object accidentally
        if (buyFeedbackText == sellFeedbackText)
        {
            Debug.LogWarning("Buy and Sell feedback texts reference the same object! Please assign separate UI elements.");
            return;
        }

        if (buyFeedbackRoutine != null)
            StopCoroutine(buyFeedbackRoutine);

        buyFeedbackText.text = message;
        buyFeedbackText.gameObject.SetActive(true);

        var dissolve = buyFeedbackText.GetComponent<TextWaterfallDissolve>();
        if (dissolve != null)
            dissolve.PlayDissolve(message);
        else
            buyFeedbackRoutine = StartCoroutine(HideAfterDelay(buyFeedbackText));
    }

    private void ShowSellFeedback(string message)
    {
        if (sellFeedbackText == null)
        {
            Debug.LogWarning("Sell feedback text not assigned!");
            return;
        }

        if (sellFeedbackText == buyFeedbackText)
        {
            Debug.LogWarning("Sell and Buy feedback texts reference the same object! Please assign separate UI elements.");
            return;
        }

        if (sellFeedbackRoutine != null)
            StopCoroutine(sellFeedbackRoutine);

        sellFeedbackText.text = message;
        sellFeedbackText.gameObject.SetActive(true);

        var dissolve = sellFeedbackText.GetComponent<TextWaterfallDissolve>();
        if (dissolve != null)
            dissolve.PlayDissolve(message);
        else
            sellFeedbackRoutine = StartCoroutine(HideAfterDelay(sellFeedbackText));
    }

    private IEnumerator HideAfterDelay(TextMeshProUGUI target)
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (target != null)
            target.gameObject.SetActive(false);
    }
}

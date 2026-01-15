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
    public TextMeshProUGUI buyFeedbackText;  // Only for Buy now
    public float feedbackDuration = 2f;

    private Coroutine buyFeedbackRoutine;

    [Header("Sell Popup Settings")]
    public GameObject sellPopupPrefab;        // Prefab for '$5 Sold!' popup
    public Transform sellPopupSpawnPoint;     // Where to spawn (e.g. near Sell button)

    [Header("Confirmation Window")]
    public GameObject confirmationWindowPrefab;
    private GameObject currentConfirmation;

    [Header("UI Canvas")]
    public Canvas targetCanvas; // Assign main UI Canvas in Inspector

    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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
            audioManager.PlaySFX(audioManager.buttonClick);

            RectTransform popupRect = currentConfirmation.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                popupRect.anchorMin = popupRect.anchorMax = popupRect.pivot = new Vector2(0.5f, 0.5f);
                popupRect.anchoredPosition = Vector2.zero;
            }
        }

        TextMeshProUGUI confirmText = currentConfirmation.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmText != null)
            confirmText.text = $"Buy 1 {flower.name} seed for ${seedCost}?";

        ConfirmationWindow window = currentConfirmation.GetComponent<ConfirmationWindow>();
        if (window != null)
        {
            window.Setup(() => ConfirmBuy(flower), CancelBuy);
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
            ShowBuyFeedback($"Bought 1 {flower?.flowerName ?? flower?.name ?? "seed"}!");
            audioManager.PlaySFX(audioManager.sellBouquet);
            BillManager.Instance.NotifyPlayerBillDue();
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
            audioManager.PlaySFX(audioManager.buttonClick);
            Destroy(currentConfirmation);
            currentConfirmation = null;
        }
    }

    // -------- BUY FEEDBACK --------
    private void ShowBuyFeedback(string message)
    {
        if (buyFeedbackText == null)
        {
            Debug.LogWarning("Buy feedback text not assigned!");
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

    private IEnumerator HideAfterDelay(TextMeshProUGUI target)
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (target != null)
            target.gameObject.SetActive(false);
    }
}

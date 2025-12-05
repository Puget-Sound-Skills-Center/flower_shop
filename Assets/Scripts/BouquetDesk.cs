using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BouquetDesk : MonoBehaviour
{
    public enum Stage { SelectFlower, Cut, Wrap, Ribbon, Complete }

    [Header("UI References")]
    public GameObject bouquetPanel;
    public TextMeshProUGUI stageText;
    public Image flowerPreview;
    public Transform flowerSelectionArea;
    public Button nextButton;
    public Button closeButton;

    [Header("Bouquet Shelf UI")]
    public Transform bouquetShelfArea;
    public GameObject bouquetDisplayPrefab;
    [Header("Shop Reference")]
    public ShopShelf shopShelf; // Assign in Inspector

    [Header("Settings")]
    public float stageDelay = 1f;

    private Stage currentStage;
    private FlowerData selectedFlower;

    private List<Button> flowerButtons = new List<Button>();

    private void Start()
    {
        if (bouquetPanel != null)
            bouquetPanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextStage);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenBouquetPanel()
    {
        if (bouquetPanel != null)
            bouquetPanel.SetActive(true);

        currentStage = Stage.SelectFlower;

        if (stageText != null)
            stageText.text = "Stage: Select a Flower";

        SetupFlowerSelection();

        // Re-enable selection buttons
        foreach (var btn in flowerButtons)
            btn.interactable = true;

        if (flowerPreview != null)
            flowerPreview.sprite = null;

        if (nextButton != null)
            nextButton.interactable = false;
    }


    private void SetupFlowerSelection()
    {
        flowerButtons.Clear();

        foreach (Transform child in flowerSelectionArea)
            Destroy(child.gameObject);

        var gm = GameManager.Instance;
        if (gm == null) return;

        var flowers = gm.GetFlowerInventory();
        foreach (var kvp in flowers)
        {
            if (kvp.Key == null || kvp.Value <= 0) continue;

            GameObject buttonObj = new GameObject($"{kvp.Key.name}_Button", typeof(RectTransform));
            buttonObj.transform.SetParent(flowerSelectionArea, false);

            var btn = buttonObj.AddComponent<Button>();
            var text = buttonObj.AddComponent<TextMeshProUGUI>();

            text.text = $"{kvp.Key.name} ({kvp.Value})";
            text.fontSize = 22;
            text.alignment = TextAlignmentOptions.Center;

            FlowerData flower = kvp.Key;
            btn.onClick.AddListener(() => SelectFlower(flower));

            // Store it
            flowerButtons.Add(btn);
        }
    }


    private void SelectFlower(FlowerData flower)
    {
        if (flower == null) return;

        selectedFlower = flower; // store the reference

        if (flowerPreview != null)
            flowerPreview.sprite = flower.bouquetDefaultSprite;

        if (stageText != null)
            stageText.text = $"Selected: {flower.flowerName}";

        if (nextButton != null)
            nextButton.interactable = true;
    }




    private void OnNextStage()
    {
        if (selectedFlower == null)
        {
            stageText.text = "Please select a flower first!";
            return;
        }

        switch (currentStage)
        {
            case Stage.SelectFlower:

                // 🔒 Now lock the buttons ONLY once transition starts
                foreach (var btn in flowerButtons)
                    btn.interactable = false;

                StartCoroutine(DoStage(Stage.Cut, "Cutting flower...", selectedFlower.bouquetCutSprite));
                break;

            case Stage.Cut:
                StartCoroutine(DoStage(Stage.Wrap, "Wrapping flower...", selectedFlower.bouquetWrappedSprite));
                break;

            case Stage.Wrap:
                StartCoroutine(DoStage(Stage.Ribbon, "Adding ribbon...", selectedFlower.bouquetRibbonSprite));
                break;

            case Stage.Ribbon:
                FinishBouquet();
                break;
        }
    }


    private IEnumerator DoStage(Stage nextStage, string message, Sprite stageSprite)
    {
        if (stageText != null)
            stageText.text = message;

        yield return new WaitForSeconds(stageDelay);

        if (flowerPreview != null)
            flowerPreview.sprite = stageSprite;

        currentStage = nextStage;

        if (stageText != null)
            stageText.text = $"Stage: {currentStage}";
    }

    private void FinishBouquet()
    {
        var gm = GameManager.Instance;

        gm.AddFlower(selectedFlower, -1); // consume flower
        gm.AddBouquet(selectedFlower);    // add bouquet to inventory

        // Send the bouquet to the real shop shelf
        if (shopShelf != null)
        {
            if (shopShelf.bouquetDisplayPrefab != null)
                shopShelf.AddBouquetToShelf(selectedFlower);
            else
                Debug.LogWarning("ShopShelf: bouquetDisplayPrefab not assigned!");
        }
        else
        {
            Debug.LogWarning("BouquetDesk: shopShelf not assigned!");
        }

        stageText.text = $"Bouquet of {selectedFlower.flowerName} completed!";

        selectedFlower = null;
        currentStage = Stage.Complete;

        StartCoroutine(CloseAfterDelay());
    }




    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        bouquetPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        bouquetPanel.SetActive(false);
    }
}

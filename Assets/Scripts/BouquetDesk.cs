using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BouquetDesk : MonoBehaviour
{
    public enum Stage { SelectFlower, Cut, Wrap, Ribbon, Review, Complete }

    [Header("UI References")]
    public GameObject bouquetPanel;
    public TextMeshProUGUI stageText;
    public Image flowerPreview;
    public Transform flowerSelectionArea;
    public Button nextButton;
    public Button closeButton;
    public FlowerData SelectedFlower => selectedFlower;

    [Header("Bouquet Shelf UI")]
    public Transform bouquetShelfArea;
    public GameObject bouquetDisplayPrefab;

    [Header("Shop Reference")]
    public ShopShelf shopShelf; // Assign in Inspector

    [Header("Settings")]
    public float stageDelay = 1f;

    public Stage currentStage;
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
        stageText.text = "Stage: Select a Flower";

        SetupFlowerSelection();

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

        foreach (var kvp in gm.GetFlowerInventory())
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

            flowerButtons.Add(btn);
        }
    }

    private void SelectFlower(FlowerData flower)
    {
        if (flower == null) return;

        selectedFlower = flower;

        if (flowerPreview != null)
            flowerPreview.sprite = flower.bouquetDefaultSprite;

        stageText.text = $"Selected: {flower.flowerName}";
        nextButton.interactable = true;
    }

    public void OnNextStage()
    {
        if (selectedFlower == null)
        {
            stageText.text = "Please select a flower first!";
            return;
        }

        switch (currentStage)
        {
            case Stage.SelectFlower:
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
                currentStage = Stage.Review;
                // ⭐ Show final bouquet sprite in review ⭐
                if (flowerPreview != null)
                    flowerPreview.sprite = selectedFlower.bouquetFinalSprite;
                stageText.text = "Stage: Review... Click preview to see tooltip.";
                break;

            case Stage.Review:
                FinishBouquet();
                break;
        }
    }

    private IEnumerator DoStage(Stage nextStage, string message, Sprite stageSprite)
    {
        stageText.text = message;
        yield return new WaitForSeconds(stageDelay);

        if (flowerPreview != null)
            flowerPreview.sprite = stageSprite;

        currentStage = nextStage;
        stageText.text = $"Stage: {currentStage}";
    }

    private void FinishBouquet()
    {
        var gm = GameManager.Instance;

        gm.AddFlower(selectedFlower, -1);
        gm.AddBouquet(selectedFlower);

        if (shopShelf != null && shopShelf.bouquetDisplayPrefab != null)
            shopShelf.AddBouquetToShelf(selectedFlower);

        stageText.text = $"Bouquet of {selectedFlower.flowerName} completed!";

        selectedFlower = null;
        currentStage = Stage.Complete;

        // Disable NEXT button; player closes manually
        nextButton.interactable = false;
    }

    public void ClosePanel()
    {
        bouquetPanel.SetActive(false);
    }
}

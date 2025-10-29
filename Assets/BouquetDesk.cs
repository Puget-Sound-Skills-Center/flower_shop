using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BouquetDesk : MonoBehaviour
{
    public enum Stage { SelectFlower, Cut, Wrap, Ribbon, PlaceOnShelf, Complete }

    [Header("UI References")]
    public GameObject bouquetPanel;
    public TextMeshProUGUI stageText;
    public Image flowerPreview;
    public Transform flowerSelectionArea;
    public Button nextButton;
    public Button closeButton;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite cutSprite;
    public Sprite wrappedSprite;
    public Sprite ribbonSprite;

    [Header("Settings")]
    public float stageDelay = 1f;

    private Stage currentStage;
    private FlowerData selectedFlower;

    private void Start()
    {
        bouquetPanel.SetActive(false);
        nextButton.onClick.AddListener(OnNextStage);
        closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenBouquetPanel()
    {
        bouquetPanel.SetActive(true);
        currentStage = Stage.SelectFlower;
        stageText.text = "Stage: Select a Flower";
        SetupFlowerSelection();
        flowerPreview.sprite = defaultSprite;
    }

    private void SetupFlowerSelection()
    {
        foreach (Transform child in flowerSelectionArea)
            Destroy(child.gameObject);

        var flowers = GameManager.Instance.GetFlowerInventory();
        foreach (var kvp in flowers)
        {
            if (kvp.Value > 0)
            {
                GameObject buttonObj = new GameObject($"{kvp.Key.name}_Button");
                buttonObj.transform.SetParent(flowerSelectionArea, false);

                var btn = buttonObj.AddComponent<Button>();
                var text = buttonObj.AddComponent<TextMeshProUGUI>();
                text.text = $"{kvp.Key.name} ({kvp.Value})";
                text.fontSize = 22;
                text.alignment = TextAlignmentOptions.Center;

                FlowerData flower = kvp.Key;
                btn.onClick.AddListener(() => SelectFlower(flower));
            }
        }
    }

    private void SelectFlower(FlowerData flower)
    {
        selectedFlower = flower;
        flowerPreview.sprite = flower.readySprite;
        stageText.text = $"Selected: {flower.name}";
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
                StartCoroutine(DoStage(Stage.Cut, "Cutting flower...", cutSprite));
                break;
            case Stage.Cut:
                StartCoroutine(DoStage(Stage.Wrap, "Wrapping flower...", wrappedSprite));
                break;
            case Stage.Wrap:
                StartCoroutine(DoStage(Stage.Ribbon, "Adding ribbon...", ribbonSprite));
                break;
            case Stage.Ribbon:
                PlaceOnShelf();
                break;
        }
    }

    private IEnumerator DoStage(Stage nextStage, string message, Sprite newSprite)
    {
        stageText.text = message;
        yield return new WaitForSeconds(stageDelay);
        flowerPreview.sprite = newSprite;
        currentStage = nextStage;
        stageText.text = $"Stage: {currentStage}";
    }

    private void PlaceOnShelf()
    {
        stageText.text = "Bouquet ready!";
        GameManager.Instance.AddBouquet(selectedFlower);
        bouquetPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        bouquetPanel.SetActive(false);
    }
}

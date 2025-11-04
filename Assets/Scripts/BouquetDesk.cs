using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

        if (flowerPreview != null)
            flowerPreview.sprite = defaultSprite;

        if (nextButton != null)
            nextButton.interactable = false;
    }

    private void SetupFlowerSelection()
    {
        if (flowerSelectionArea == null) return;

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
        }
    }

    private void SelectFlower(FlowerData flower)
    {
        selectedFlower = flower;
        if (flowerPreview != null && flower != null)
            flowerPreview.sprite = flower.readySprite;

        if (stageText != null && flower != null)
            stageText.text = $"Selected: {flower.name}";

        if (nextButton != null)
            nextButton.interactable = true;
    }

    private void OnNextStage()
    {
        if (selectedFlower == null)
        {
            if (stageText != null)
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
                FinishBouquet(); // Skip "PlaceOnShelf"
                break;
            case Stage.Complete:
                if (stageText != null)
                    stageText.text = "Bouquet complete!";
                break;
        }
    }

    private IEnumerator DoStage(Stage nextStage, string message, Sprite newSprite)
    {
        if (stageText != null)
            stageText.text = message;

        yield return new WaitForSeconds(stageDelay);

        if (flowerPreview != null && newSprite != null)
            flowerPreview.sprite = newSprite;

        currentStage = nextStage;
        if (stageText != null)
            stageText.text = $"Stage: {currentStage}";
    }

    private void FinishBouquet()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("BouquetDesk: GameManager instance missing!");
            return;
        }

        // ✅ Remove one flower from inventory (consumed for bouquet)
        gm.AddFlower(selectedFlower, -1);

        // ✅ Add the finished bouquet to the bouquet inventory
        gm.AddBouquet(selectedFlower);

        // ✅ Update the bouquet shelf UI to reflect the new addition
        UpdateBouquetShelfUI();

        if (stageText != null)
            stageText.text = $"Bouquet of {selectedFlower.name} completed!";

        // Reset selection
        selectedFlower = null;
        currentStage = Stage.Complete;

        // Optionally auto-close after a delay
        StartCoroutine(CloseAfterDelay());
    }

    private void UpdateBouquetShelfUI()
    {
        if (bouquetShelfArea == null || bouquetDisplayPrefab == null)
        {
            Debug.LogWarning("BouquetDesk: shelfArea or prefab not assigned.");
            return;
        }

        foreach (Transform child in bouquetShelfArea)
            Destroy(child.gameObject);

        foreach (var kvp in GameManager.Instance.GetBouquetInventory())
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                GameObject bouquetObj = Instantiate(bouquetDisplayPrefab, bouquetShelfArea);
                var img = bouquetObj.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = kvp.Key.readySprite;
                    img.enabled = true;
                }
            }
        }
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (bouquetPanel != null)
            bouquetPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        if (bouquetPanel != null)
            bouquetPanel.SetActive(false);
    }
}

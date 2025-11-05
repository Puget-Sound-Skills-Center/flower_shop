using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ComputerPanelManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject computerPanel;
    public CanvasGroup panelCanvasGroup;
    public Button closeButton;

    [Header("Tab Buttons")]
    public Button seedsTabButton;
    public Button potsTabButton;

    [Header("Tab Content Areas")]
    public Transform seedsTabContent;
    public Transform potsTabContent;

    [Header("Prefabs")]
    public GameObject shopItemButtonPrefab;

    [Header("Shop Data")]
    public List<FlowerData> availableSeeds; // Use FlowerData for seeds
    public int potCost = 10;
    public int maxPotsToBuy = 10;

    [Header("Animation Settings")]
    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private Coroutine fadeRoutine;

    private enum Tab { Seeds, Pots }
    private Tab currentTab;

    private void Awake()
    {
        if (computerPanel != null)
            computerPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        if (seedsTabButton != null)
            seedsTabButton.onClick.AddListener(() => SwitchTab(Tab.Seeds));

        if (potsTabButton != null)
            potsTabButton.onClick.AddListener(() => SwitchTab(Tab.Pots));
    }

    public void OpenPanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        computerPanel.SetActive(true);

        if (panelCanvasGroup != null)
            fadeRoutine = StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, 1f));

        isOpen = true;
        SwitchTab(Tab.Seeds);
    }

    public void ClosePanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (panelCanvasGroup != null)
            fadeRoutine = StartCoroutine(FadeAndDeactivate(panelCanvasGroup));
        else
            computerPanel.SetActive(false);

        isOpen = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end)
    {
        float t = 0f;
        group.alpha = start;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
        group.alpha = end;
    }

    private IEnumerator FadeAndDeactivate(CanvasGroup group)
    {
        yield return FadeCanvasGroup(group, 1f, 0f);
        computerPanel.SetActive(false);
    }

    // -------------------------------
    // 🧩 TAB SWITCHING
    // -------------------------------
    private void SwitchTab(Tab tab)
    {
        currentTab = tab;
        bool isSeeds = (tab == Tab.Seeds);

        if (seedsTabContent != null)
            seedsTabContent.gameObject.SetActive(isSeeds);

        if (potsTabContent != null)
            potsTabContent.gameObject.SetActive(!isSeeds);

        if (seedsTabButton != null)
            HighlightTab(seedsTabButton, isSeeds);
        if (potsTabButton != null)
            HighlightTab(potsTabButton, !isSeeds);

        if (isSeeds)
            PopulateSeedShop();
        else
            PopulatePotShop();
    }

    private void HighlightTab(Button button, bool isActive)
    {
        var colors = button.colors;
        colors.normalColor = isActive ? new Color(0.8f, 1f, 0.8f) : Color.white;
        button.colors = colors;
    }

    // -------------------------------
    // 🌱 SEED SHOP
    // -------------------------------
    private void PopulateSeedShop()
    {
        if (seedsTabContent == null || shopItemButtonPrefab == null)
        {
            Debug.LogWarning("ComputerPanelManager: Missing references for Seed Shop.");
            return;
        }

        foreach (Transform child in seedsTabContent)
            Destroy(child.gameObject);

        foreach (var flower in availableSeeds)
        {
            GameObject buttonObj = Instantiate(shopItemButtonPrefab, seedsTabContent);
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            var button = buttonObj.GetComponent<Button>();

            text.text = $"{flower.name} Seeds - ${flower.seedCost}";
            button.onClick.AddListener(() => TryBuySeed(flower));
        }
    }

    private void TryBuySeed(FlowerData flower)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.money >= flower.seedCost)
        {
            gm.AddMoney(-flower.seedCost);
            gm.AddFlower(flower, 1); // Add one seed/flower
            Debug.Log($"Bought seed: {flower.name}");
        }
        else
        {
            Debug.Log("Not enough money to buy seed!");
        }
    }

    // -------------------------------
    // 🏺 POT SHOP
    // -------------------------------
    private void PopulatePotShop()
    {
        if (potsTabContent == null || shopItemButtonPrefab == null)
        {
            Debug.LogWarning("ComputerPanelManager: Missing references for Pot Shop.");
            return;
        }

        foreach (Transform child in potsTabContent)
            Destroy(child.gameObject);

        for (int i = 1; i <= maxPotsToBuy; i++)
        {
            int totalCost = i * potCost;

            GameObject potItemObj = Instantiate(shopItemButtonPrefab, potsTabContent);
            var potItem = potItemObj.GetComponent<PotShopItem>();

            if (potItem != null)
            {
                potItem.SetupItem(i, totalCost);
            }
            else
            {
                Debug.LogWarning("PotShopItem prefab missing PotShopItem component!");
            }
        }
    }


    private void TryBuyPots(int count, int cost)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.money >= cost)
        {
            gm.AddMoney(-cost);
            gm.AddPots(count);
            Debug.Log($"Bought {count} pots for ${cost}");
        }
        else
        {
            Debug.Log("Not enough money to buy pots!");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ComputerPanelManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject computerPanel;
    public CanvasGroup panelCanvasGroup;
    public Button closeButton;

    [Header("Tabs")]
    public GameObject seedTabContent;
    public GameObject potsTabContent;
    public Button seedTabButton;
    public Button potsTabButton;

    [Header("Tab Panel Sprite")]
    public Image panelBackgroundImage; // Assign the panel's Image component here
    public Sprite seedTabSprite;       // Sprite for the seeds tab
    public Sprite potsTabSprite;       // Sprite for the pots tab

    [Header("Growing Area Reference")]
    public GrowingAreaManager growingAreaManager;
    // Assign the GrowingAreaManager from your scene here

    [Header("Animation Settings")]
    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private Coroutine fadeRoutine;
    private bool showingSeeds = true;

    private void Awake()
    {
        if (computerPanel != null)
            computerPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        if (seedTabButton != null)
        {
            seedTabButton.onClick.RemoveAllListeners();
            seedTabButton.onClick.AddListener(() => SwitchTab(true));
        }

        if (potsTabButton != null)
        {
            potsTabButton.onClick.RemoveAllListeners();
            potsTabButton.onClick.AddListener(() => SwitchTab(false));
        }

        // Start on the seeds tab
        SwitchTab(true);
    }

    public void OpenPanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        computerPanel.SetActive(true);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            fadeRoutine = StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, 1f));
        }

        isOpen = true;
        SwitchTab(true);
    }

    public void ClosePanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
            fadeRoutine = StartCoroutine(FadeAndDeactivate(panelCanvasGroup));
        }
        else
        {
            computerPanel.SetActive(false);
        }

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

    // 🌱 TAB SWITCHING
    private void SwitchTab(bool showSeeds)
    {
        showingSeeds = showSeeds;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Hide all tab content first
        if (seedTabContent != null) seedTabContent.SetActive(false);
        if (potsTabContent != null) potsTabContent.SetActive(false);

        // Show selected tab
        if (showSeeds)
        {
            seedTabContent?.SetActive(true);
            if (panelBackgroundImage != null && seedTabSprite != null)
                panelBackgroundImage.sprite = seedTabSprite;
        }
        else
        {
            potsTabContent?.SetActive(true);
            if (panelBackgroundImage != null && potsTabSprite != null)
                panelBackgroundImage.sprite = potsTabSprite;

            // 🪴 Assign GrowingAreaManager to all PotShopItem prefabs currently in the PotsTabContent
            if (growingAreaManager != null && potsTabContent != null)
            {
                foreach (Transform child in potsTabContent.transform)
                {
                    PotShopItem shopItem = child.GetComponent<PotShopItem>();
                    if (shopItem != null)
                    {
                        shopItem.growingAreaManager = growingAreaManager;
                    }
                }
            }
            else
            {
                Debug.LogWarning("ComputerPanelManager: GrowingAreaManager or potsTabContent not assigned.");
            }
        }

        HighlightTabButton(seedTabButton, showSeeds);
        HighlightTabButton(potsTabButton, !showSeeds);
    }

    private void HighlightTabButton(Button button, bool active)
    {
        if (button == null) return;

        var colors = button.colors;
        Color highlight = new Color(0.8f, 0.9f, 1f);
        Color selected = new Color(0.7f, 0.85f, 1f);
        colors.normalColor = active ? highlight : Color.white;
        colors.selectedColor = active ? selected : Color.white;
        colors.pressedColor = active ? highlight : Color.white;
        colors.highlightedColor = active ? highlight : Color.white;
        button.colors = colors;

        var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            text.color = active ? new Color(0.2f, 0.5f, 1f) : Color.black;
    }
}

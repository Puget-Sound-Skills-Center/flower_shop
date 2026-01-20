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

    public BillManager billManager;

    [SerializeField] private PanelTab currentTab;

    [Header("Tabs")]
    public GameObject seedTabContent;
    public GameObject potsTabContent;
    public GameObject billTabContent;
    public Button seedTabButton;
    public Button potsTabButton;
    public Button billsTabButton;

    [Header("Tab Panel Sprite")]
    public Image panelBackgroundImage; // Assign the panel's Image component here
    public Sprite seedTabSprite;       // Sprite for the seeds tab
    public Sprite potsTabSprite;       // Sprite for the pots tab
    public Sprite billsTabSprite;

    [Header("Growing Area Reference")]
    public GrowingAreaManager growingAreaManager;
    // Assign the GrowingAreaManager from your scene here

    [Header("Animation Settings")]
    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private Coroutine fadeRoutine;

    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

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
            seedTabButton.onClick.AddListener(() => SwitchTab(PanelTab.Seeds));
        }

        if (potsTabButton != null)
        {
            potsTabButton.onClick.RemoveAllListeners();
            potsTabButton.onClick.AddListener(() => SwitchTab(PanelTab.Pots));
        }
        if (billsTabButton != null)
        {
            billsTabButton.onClick.RemoveAllListeners();
            billsTabButton.onClick.AddListener(() => SwitchTab(PanelTab.Bills));
        }

        // Start on the seeds tab
        SwitchTab(PanelTab.Seeds);
    }

    public void OpenPanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        computerPanel.SetActive(true);
        audioManager.PlaySFX(audioManager.buttonClick);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            fadeRoutine = StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, 1f));
        }

        isOpen = true;
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
            audioManager.PlaySFX(audioManager.buttonClick);
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

    // Panel Tab Enum
    public enum PanelTab
    {
        Seeds,
        Pots,
        Bills
    }

    // 🌱 TAB SWITCHING
    public void SwitchTab(PanelTab tab)
    {
        currentTab = tab;

        // Disable all tab contents first
        seedTabContent.SetActive(false);
        potsTabContent.SetActive(false);
        billTabContent.SetActive(false);

        // Enable selected tab + swap sprite
        switch (tab)
        {
            case PanelTab.Seeds:
                seedTabContent.SetActive(true);
                panelBackgroundImage.sprite = seedTabSprite;
                audioManager.PlaySFX(audioManager.buttonClick);
                break;

            case PanelTab.Pots:
                potsTabContent.SetActive(true);
                panelBackgroundImage.sprite = potsTabSprite;
                audioManager.PlaySFX(audioManager.buttonClick);
                break;

            case PanelTab.Bills:
                billTabContent.SetActive(true);
                panelBackgroundImage.sprite = billsTabSprite;
                audioManager.PlaySFX(audioManager.buttonClick);
                // billManager.setRentText();
                break;
        }
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

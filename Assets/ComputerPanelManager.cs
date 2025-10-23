using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComputerPanelManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject computerPanel;      // Assign your ComputerPanel UI object
    public CanvasGroup panelCanvasGroup;  // Optional (for fade-in/out)
    public Button closeButton;            // The Close button inside the panel

    [Header("Animation Settings")]
    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (computerPanel != null)
            computerPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    // Called by clicking on the Computer sprite
    public void OpenPanel()
    {
        if (computerPanel == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        computerPanel.SetActive(true);
        if (panelCanvasGroup != null)
        {
            fadeRoutine = StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, 1f));
        }

        isOpen = true;
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
}

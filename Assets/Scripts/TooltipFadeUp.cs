using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipFadeUp : MonoBehaviour
{
    [Header("Animation Settings")]
    public float fadeInDuration = 0.15f;
    public float fadeOutDuration = 0.12f;
    public float riseDistance = 12f;

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private bool isClosing = false;

    private Vector2 finalPos;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        finalPos = rect.anchoredPosition;
    }

    private void OnEnable()
    {
        PlayFadeIn();
    }

    // -------------------- OPEN --------------------

    public void PlayFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float t = 0f;

        canvasGroup.alpha = 0f;
        rect.anchoredPosition = finalPos - new Vector2(0f, riseDistance);

        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeInDuration);

            canvasGroup.alpha = k;
            rect.anchoredPosition =
                Vector2.Lerp(finalPos - new Vector2(0f, riseDistance), finalPos, k);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rect.anchoredPosition = finalPos;
    }

    // -------------------- CLOSE --------------------

    public void PlayFadeOutAndDestroy()
    {
        if (isClosing) return;

        isClosing = true;
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = finalPos - new Vector2(0f, riseDistance);

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeOutDuration);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, k);
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, k);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}
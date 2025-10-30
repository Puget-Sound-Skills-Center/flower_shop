using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWaterfallDissolve : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    [Header("Timing Settings")]
    public float letterDelay = 0.03f;    // Delay between each character reveal
    public float fadeOutSpeed = 1f;      // Speed of fade out
    public float holdDuration = 1f;      // How long the text stays visible before fading out

    [Header("Color Settings")]
    public Color successColor = new Color(0.3f, 1f, 0.3f); // light green
    public Color errorColor = new Color(1f, 0.3f, 0.3f);   // light red
    public Color defaultColor = Color.white;

    private Coroutine currentRoutine;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.alpha = 0f;
    }

    /// <summary>
    /// Plays a waterfall dissolve animation for the given text, auto-coloring if it detects success/error keywords.
    /// </summary>
    public void PlayDissolve(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        // Auto-color based on message content
        if (message.ToLower().Contains("sold"))
            tmp.color = successColor;
        else if (message.ToLower().Contains("no"))
            tmp.color = errorColor;
        else
            tmp.color = defaultColor;

        tmp.text = message;
        tmp.alpha = 1f;
        tmp.maxVisibleCharacters = 0;
        gameObject.SetActive(true);

        currentRoutine = StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        tmp.ForceMeshUpdate();
        int totalChars = tmp.textInfo.characterCount;

        // Step 1: Reveal text letter by letter
        for (int i = 0; i < totalChars; i++)
        {
            tmp.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(letterDelay);
        }

        // Step 2: Hold before fade-out
        yield return new WaitForSeconds(holdDuration);

        // Step 3: Fade out smoothly
        float fade = 1f;
        while (fade > 0)
        {
            fade -= Time.deltaTime * fadeOutSpeed;
            tmp.alpha = fade;
            yield return null;
        }

        tmp.alpha = 0f;
        gameObject.SetActive(false);
    }
}

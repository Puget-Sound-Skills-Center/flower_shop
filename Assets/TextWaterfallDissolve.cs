using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWaterfallDissolve : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    public float letterDelay = 0.03f;    // Delay between each letter dissolving
    public float fadeOutSpeed = 1f;      // Speed of fade-out after fully revealed

    private Coroutine currentRoutine;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Plays a waterfall dissolve animation for the given text.
    /// </summary>
    public void PlayDissolve(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        tmp.text = message;
        tmp.alpha = 1f; // ensure visible
        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        // Step 1: Animate a reveal effect letter by letter
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            tmp.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(letterDelay);
        }

        // Step 2: Wait briefly before fading out
        yield return new WaitForSeconds(1f);

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

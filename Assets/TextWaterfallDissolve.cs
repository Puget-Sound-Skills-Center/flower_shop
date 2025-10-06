using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWaterfallDissolve : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    private Coroutine dissolveRoutine;

    [Header("Animation Settings")]
    public float dissolveDuration = 1.5f; // seconds for full dissolve
    public float holdDuration = 1f;       // delay before starting dissolve
    public bool topToBottom = true;       // direction of dissolve

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void PlayDissolve(string message)
    {
        if (dissolveRoutine != null)
            StopCoroutine(dissolveRoutine);

        tmpText.text = message;
        tmpText.ForceMeshUpdate(); // Ensure we have geometry
        tmpText.alpha = 1f; // Make fully visible
        tmpText.gameObject.SetActive(true);

        dissolveRoutine = StartCoroutine(DissolveEffect());
    }

    private IEnumerator DissolveEffect()
    {
        yield return new WaitForSeconds(holdDuration);

        tmpText.ForceMeshUpdate();
        var textInfo = tmpText.textInfo;

        // Find min/max Y positions to compute direction
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            minY = Mathf.Min(minY, charInfo.bottomLeft.y);
            maxY = Mathf.Max(maxY, charInfo.topLeft.y);
        }

        float timer = 0f;
        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / dissolveDuration;

            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int matIndex = charInfo.materialReferenceIndex;
                int vertIndex = charInfo.vertexIndex;
                var colors = textInfo.meshInfo[matIndex].colors32;

                float relativeY = Mathf.InverseLerp(minY, maxY, charInfo.topLeft.y);
                if (!topToBottom) relativeY = 1f - relativeY;

                float alphaFactor = Mathf.Clamp01((progress * 1.2f) - relativeY); // dissolve wave
                byte alpha = (byte)(255 * (1f - alphaFactor));

                for (int j = 0; j < 4; j++)
                    colors[vertIndex + j].a = alpha;
            }

            // Push updates to the mesh
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }

        tmpText.gameObject.SetActive(false);
        dissolveRoutine = null;
    }
}

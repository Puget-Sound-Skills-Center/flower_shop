using UnityEngine;
using TMPro;
using System.Collections;

public class SellPopup : MonoBehaviour
{
    public float floatSpeed = 40f;
    public float duration = 1.2f;
    private TextMeshProUGUI text;
    private Color startColor;
    private RectTransform rect;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        startColor = text.color;
    }

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = rect.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.localPosition = startPos + Vector3.up * floatSpeed * (elapsed / duration);

            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}

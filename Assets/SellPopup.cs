using UnityEngine;
using TMPro;

public class SellPopup : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float duration = 1.2f;
    public Vector3 moveDirection = new Vector3(0, 1, 0);
    private TextMeshProUGUI text;
    private Color startColor;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text != null) startColor = text.color;
    }

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private System.Collections.IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = startPos + moveDirection * floatSpeed * (elapsed / duration);
            if (text != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}

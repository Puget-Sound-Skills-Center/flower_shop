using UnityEngine;
using TMPro;

public class HarvestPopup : MonoBehaviour
{
    public float floatSpeed = 50f;
    public float lifetime = 1f;
    public TextMeshProUGUI text;

    private CanvasGroup canvasGroup;
    private float timer;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
            if (text == null)
                Debug.LogWarning("HarvestPopup: TextMeshProUGUI reference is missing.");
        }
    }

    private void Update()
    {
        // Move upwards
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // Fade out over time
        timer += Time.deltaTime;
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Clamp01(1 - (timer / lifetime));

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}


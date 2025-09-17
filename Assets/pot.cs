using UnityEngine;
using TMPro;
using System.Collections;

public class Pot : MonoBehaviour
{
    [Header("Plant Growth Settings")]
    public float growTime = 20f;
    public Sprite emptyPotSprite;
    public Sprite[] growthStages;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private SpriteRenderer spriteRenderer;
    private bool isGrowing = false;
    private float timer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = emptyPotSprite;
        timerText.text = "";
    }

    private void OnMouseDown()
    {
        // Plant only if we have seeds and pot is empty
        if (!isGrowing && GameManager.Instance.seedCount > 0)
        {
            GameManager.Instance.AddSeed(-1);
            StartCoroutine(GrowPlant());
        }
    }

    IEnumerator GrowPlant()
    {
        isGrowing = true;
        timer = growTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString() + "s";

            float progress = 1f - (timer / growTime);
            int stageIndex = Mathf.FloorToInt(progress * growthStages.Length);
            stageIndex = Mathf.Clamp(stageIndex, 0, growthStages.Length - 1);

            spriteRenderer.sprite = growthStages[stageIndex];

            yield return null;
        }

        // Growth complete
        timerText.text = "";
        spriteRenderer.sprite = emptyPotSprite; // reset to empty
        isGrowing = false;

        // Add flower to inventory
        GameManager.Instance.AddFlower(1);
    }
}

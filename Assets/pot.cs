using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Pot : MonoBehaviour
{
    [Header("Growth settings")]
    public float growTime = 20f;
    public Sprite emptyPotSprite;
    public Sprite[] growthStages; // from sprout → full grown

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    private SpriteRenderer spriteRenderer;
    private Coroutine growRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";
    }

    private void OnEnable()
    {
        // 🔄 Always refresh UI on room re-entry
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateAllUI();

            if (GameManager.Instance.potIsGrowing)
            {
                StartGrowthRoutine();
            }
            else
            {
                spriteRenderer.sprite = emptyPotSprite;
                if (timerText != null) timerText.text = "";
            }
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.potIsGrowing) return;   // already growing
        if (GameManager.Instance.seedCount <= 0) return; // no seeds

        // Use one seed
        GameManager.Instance.AddSeed(-1);

        // Start pot growth in GameManager
        GameManager.Instance.StartPotGrowth(growTime);

        // Start visuals
        StartGrowthRoutine();
    }

    private void StartGrowthRoutine()
    {
        if (growRoutine != null) StopCoroutine(growRoutine);
        growRoutine = StartCoroutine(GrowthCoroutine());
    }

    private IEnumerator GrowthCoroutine()
    {
        while (true)
        {
            if (GameManager.Instance == null) break;

            // Ask GameManager for remaining time
            if (GameManager.Instance.GetPotState(out float remaining, out float totalGrow))
            {
                // Update timer
                if (timerText != null) timerText.text = Mathf.Ceil(remaining).ToString() + "s";

                // Update sprite based on progress
                float progress = 1f - Mathf.Clamp01(remaining / totalGrow);
                if (growthStages != null && growthStages.Length > 0)
                {
                    int stageIndex = Mathf.FloorToInt(progress * growthStages.Length);
                    stageIndex = Mathf.Clamp(stageIndex, 0, growthStages.Length - 1);
                    spriteRenderer.sprite = growthStages[stageIndex];
                }
            }
            else
            {
                // Growth finished
                GameManager.Instance.AddFlower(1); // ✅ give flower to player

                // Reset visuals
                spriteRenderer.sprite = emptyPotSprite;
                if (timerText != null) timerText.text = "";

                break;
            }

            yield return null;
        }

        growRoutine = null;
    }
}

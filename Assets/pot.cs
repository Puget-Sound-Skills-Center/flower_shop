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
        if (spriteRenderer == null)
        {
            Debug.LogError("Pot: SpriteRenderer component missing.");
            enabled = false;
            return;
        }
        if (emptyPotSprite != null)
            spriteRenderer.sprite = emptyPotSprite;
        else
            Debug.LogWarning("Pot: emptyPotSprite not assigned.");
        if (timerText != null)
            timerText.text = "";
        else
            Debug.LogWarning("Pot: timerText UI reference is missing.");
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.potIsGrowing)
        {
            StartGrowthRoutine();
        }
    }

    private void OnMouseDown()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("Pot: GameManager instance is missing.");
            return;
        }
        if (gm.potIsGrowing) return;
        if (gm.seedCount <= 0)
        {
            Debug.Log("Pot: Not enough seeds to plant.");
            return;
        }

        gm.AddSeed(-1);
        gm.StartPotGrowth(growTime);
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
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("Pot: GameManager instance is missing during growth.");
                break;
            }

            if (gm.GetPotState(out float remaining, out float totalGrow))
            {
                if (timerText != null)
                    timerText.text = Mathf.Ceil(remaining).ToString() + "s";

                float progress = 1f - Mathf.Clamp01(remaining / totalGrow);
                if (growthStages != null && growthStages.Length > 0)
                {
                    int stageIndex = Mathf.FloorToInt(progress * growthStages.Length);
                    stageIndex = Mathf.Clamp(stageIndex, 0, growthStages.Length - 1);
                    if (growthStages[stageIndex] != null)
                        spriteRenderer.sprite = growthStages[stageIndex];
                    else
                        Debug.LogWarning($"Pot: growthStages[{stageIndex}] is not assigned.");
                }
            }
            else
            {
                spriteRenderer.sprite = emptyPotSprite;
                if (timerText != null) timerText.text = "";
                break;
            }

            yield return null;
        }

        growRoutine = null;
    }
}

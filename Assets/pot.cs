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
    public Sprite readyToHarvestSprite; // 🌼 sprite when ready

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Harvest Popup")]
    public GameObject harvestPopupPrefab;
    public Transform popupSpawnPoint; // Optional: where popup spawns

    private SpriteRenderer spriteRenderer;
    private Coroutine growRoutine;

    private bool isGrowing = false;
    private bool readyToHarvest = false;

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
        if (readyToHarvestSprite == null)
            Debug.LogWarning("Pot: readyToHarvestSprite not assigned.");
        if (harvestPopupPrefab == null)
            Debug.LogWarning("Pot: harvestPopupPrefab not assigned.");
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.potIsGrowing)
        {
            StartGrowthRoutine();
        }
        else
        {
            isGrowing = false;
            readyToHarvest = false;
            if (spriteRenderer != null && emptyPotSprite != null)
                spriteRenderer.sprite = emptyPotSprite;
            if (timerText != null) timerText.text = "";
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

        if (readyToHarvest)
        {
            HarvestFlower();
            return;
        }

        if (isGrowing || gm.potIsGrowing) return;

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
        isGrowing = true;
        readyToHarvest = false;

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
                isGrowing = false;
                readyToHarvest = true;

                if (readyToHarvestSprite != null)
                    spriteRenderer.sprite = readyToHarvestSprite;
                else if (spriteRenderer != null && emptyPotSprite != null)
                    spriteRenderer.sprite = emptyPotSprite;

                if (timerText != null) timerText.text = "!";

                break;
            }

            yield return null;
        }

        growRoutine = null;
    }

    private void HarvestFlower()
    {
        readyToHarvest = false;
        isGrowing = false;
        if (spriteRenderer != null && emptyPotSprite != null)
            spriteRenderer.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";

        var gm = GameManager.Instance;
        if (gm != null)
            gm.AddFlower(1);
        else
            Debug.LogError("Pot: GameManager instance is missing during harvest.");

        // 🌟 Spawn floating popup
        if (harvestPopupPrefab != null)
        {
            Transform spawn = popupSpawnPoint != null ? popupSpawnPoint : transform;
            GameObject popup = Instantiate(harvestPopupPrefab, spawn.position, Quaternion.identity);

            // Ensure it appears inside UI Canvas
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                popup.transform.SetParent(canvas.transform, false);
            }
            else
            {
                Debug.LogWarning("Pot: No Canvas found for popup.");
            }
        }
    }
}

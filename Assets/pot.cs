using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Pot : MonoBehaviour
{
    [Header("Pot Sprites")]
    public Sprite emptyPotSprite; // Assign in Inspector

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Harvest Popup")]
    public GameObject harvestPopupPrefab;
    public Transform popupSpawnPoint; // Optional: where popup spawns

    private SpriteRenderer spriteRenderer;
    private Coroutine growRoutine;

    private bool isGrowing = false;
    private bool readyToHarvest = false;

    public FlowerData currentFlower; // The flower this pot is growing

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Pot: SpriteRenderer component missing.");
            enabled = false;
            return;
        }
        spriteRenderer.enabled = true;
        if (emptyPotSprite != null)
            spriteRenderer.sprite = emptyPotSprite;
        else
            Debug.LogWarning("Pot: emptyPotSprite not assigned.");
        if (timerText != null)
            timerText.text = "";
        else
            Debug.LogWarning("Pot: timerText UI reference is missing.");
        if (harvestPopupPrefab == null)
            Debug.LogWarning("Pot: harvestPopupPrefab not assigned.");
    }

    private void OnEnable()
    {
        isGrowing = false;
        readyToHarvest = false;
        if (spriteRenderer != null && emptyPotSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = emptyPotSprite;
        }
        if (timerText != null) timerText.text = "";
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

        if (isGrowing) return;

        if (gm.seedCount <= 0)
        {
            Debug.Log("Pot: Not enough seeds to plant.");
            return;
        }

        if (gm.selectedFlower == null)
        {
            Debug.LogWarning("Pot: No flower selected to plant.");
            return;
        }

        gm.AddSeed(-1);
        currentFlower = gm.selectedFlower;
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

        float elapsed = 0f;
        float growTime = currentFlower != null ? currentFlower.growTime : 20f;
        Sprite[] stages = currentFlower != null ? currentFlower.growthStages : null;

        while (elapsed < growTime)
        {
            elapsed += Time.deltaTime;

            if (timerText != null)
                timerText.text = Mathf.Ceil(growTime - elapsed) + "s";

            // Update sprite stage
            if (spriteRenderer != null && stages != null && stages.Length > 0)
            {
                float progress = elapsed / growTime;
                int stageIndex = Mathf.FloorToInt(progress * stages.Length);
                stageIndex = Mathf.Clamp(stageIndex, 0, stages.Length - 1);
                if (stages[stageIndex] != null)
                {
                    spriteRenderer.enabled = true;
                    spriteRenderer.sprite = stages[stageIndex];
                }
                else
                {
                    Debug.LogWarning($"Pot: growthStages[{stageIndex}] is not assigned for flower '{currentFlower.flowerName}'.");
                }
            }
            else if (spriteRenderer != null && emptyPotSprite != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.sprite = emptyPotSprite;
            }

            yield return null;
        }

        isGrowing = false;
        readyToHarvest = true;

        if (spriteRenderer != null && currentFlower != null && currentFlower.readySprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = currentFlower.readySprite;
        }
        else if (spriteRenderer != null && emptyPotSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = emptyPotSprite;
        }
        if (timerText != null) timerText.text = "Ready!";
        growRoutine = null;
    }

    private void HarvestFlower()
    {
        readyToHarvest = false;
        isGrowing = false;
        if (spriteRenderer != null && emptyPotSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = emptyPotSprite;
        }
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

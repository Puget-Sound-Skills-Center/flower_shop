using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Pot : MonoBehaviour
{
    [Header("Pot Sprites")]
    public Sprite emptyPotSprite; // Assign in Inspector
    public Sprite sproutSprite;   // Assign in Inspector (can be same as growthStages[0] or a unique sprout)

    [Header("Growth Timing")]
    [Range(0f, 2f)]
    public float emptyToSproutTime = 0.5f; // Seconds to show empty pot before sprout
    [Range(0.05f, 0.5f)]
    public float sproutPercent = 0.2f; // % of growTime spent in sprout stage

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
        if (gm == null) return;

        if (readyToHarvest)
        {
            HarvestFlower();
            return;
        }

        if (isGrowing) return;

        if (gm.selectedFlower == null)
        {
            Debug.LogWarning("Pot: No flower selected to plant.");
            return;
        }

        // ✅ Check seed count before planting
        if (!gm.UseSeed(gm.selectedFlower))
        {
            Debug.Log("Pot: Not enough seeds of " + gm.selectedFlower.flowerName);
            return;
        }
    }


    private IEnumerator GrowthCoroutine()
    {
        isGrowing = true;
        readyToHarvest = false;

        float elapsed = 0f;
        float growTime = currentFlower != null ? currentFlower.growTime : 20f;
        Sprite[] stages = currentFlower != null ? currentFlower.growthStages : null;

        // Show empty pot at the very start
        if (spriteRenderer != null && emptyPotSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = emptyPotSprite;
        }

        // Show empty pot for adjustable time
        if (emptyToSproutTime > 0f)
            yield return new WaitForSeconds(emptyToSproutTime);

        // Show sprout for adjustable percent of growTime
        float sproutTime = growTime * sproutPercent;
        float sproutElapsed = 0f;
        if (sproutSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sproutSprite;
        }
        else if (stages != null && stages.Length > 0 && stages[0] != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = stages[0];
        }
        else if (emptyPotSprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = emptyPotSprite;
        }

        while (sproutElapsed < sproutTime)
        {
            sproutElapsed += Time.deltaTime;
            elapsed += Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.Ceil(growTime - elapsed) + "s";
            yield return null;
        }

        // Transition through remaining growth stages
        float remainingTime = growTime - sproutTime;
        float stageElapsed = 0f;
        int numStages = stages != null ? stages.Length : 0;
        while (stageElapsed < remainingTime)
        {
            stageElapsed += Time.deltaTime;
            elapsed += Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.Ceil(growTime - elapsed) + "s";

            if (spriteRenderer != null && numStages > 1)
            {
                // Interpolate from stage 1 to last
                float stageProgress = stageElapsed / remainingTime;
                int stageIndex = 1 + Mathf.FloorToInt(stageProgress * (numStages - 2));
                stageIndex = Mathf.Clamp(stageIndex, 1, numStages - 1);
                if (stages[stageIndex] != null)
                {
                    spriteRenderer.enabled = true;
                    spriteRenderer.sprite = stages[stageIndex];
                }
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
        if (timerText != null) timerText.text = "!";
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

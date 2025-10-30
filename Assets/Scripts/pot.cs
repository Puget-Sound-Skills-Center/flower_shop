using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Pot : MonoBehaviour
{
    [Header("Pot Sprites")]
    public Sprite emptyPotSprite;
    public Sprite sproutSprite;

    [Header("Growth Timing")]
    [Tooltip("Seconds to show empty pot before sprout")]
    public float emptyToSproutTime = 0.5f;
    [Tooltip("Percent (0-1) of growTime spent in sprout stage")]
    [Range(0f, 1f)]
    public float sproutPercent = 0.2f;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Harvest Popup")]
    public GameObject harvestPopupPrefab;
    public Transform popupSpawnPoint;
    [Tooltip("Optional: assign the Canvas to parent harvest popups to. If null code will attempt to FindObjectOfType<Canvas>().")]
    public Canvas targetCanvas;

    private Image potImage;                       // UI Image used for pot visuals
    private Coroutine growRoutine;
    private bool isGrowing = false;
    private bool readyToHarvest = false;
    private FlowerData currentFlower;

    private void Awake()
    {
        potImage = GetComponentInChildren<Image>();
        if (potImage == null)
        {
            Debug.LogError($"Pot ({name}): No UI Image found in children. Disabling Pot component.");
            enabled = false;
            return;
        }

        // initialize visuals
        potImage.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";

        // ensure values are sane
        sproutPercent = Mathf.Clamp01(sproutPercent);
        emptyToSproutTime = Mathf.Max(0f, emptyToSproutTime);
    }

    private void OnValidate()
    {
        // editor-time clamping to avoid invalid inspector values
        sproutPercent = Mathf.Clamp01(sproutPercent);
        emptyToSproutTime = Mathf.Max(0f, emptyToSproutTime);
    }

    /// <summary>
    /// Called by UI Button OnClick() (hook this in the Button component).
    /// If pot is ready to harvest it will harvest; otherwise it will attempt to plant.
    /// </summary>
    public void OnGrowButtonClick()
    {
        if (!enabled || potImage == null)
        {
            Debug.LogWarning($"Pot ({name}): OnGrowButtonClick called but component is disabled or missing Image.");
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("Pot: GameManager instance missing.");
            return;
        }

        if (readyToHarvest)
        {
            HarvestFlower();
            return;
        }

        if (isGrowing)
        {
            Debug.Log("Pot: Already growing.");
            return;
        }

        // select flower to plant (prefers selectedFlower, falls back to any available)
        FlowerData flowerToPlant = gm.GetPlantableFlower(gm.selectedFlower);
        if (flowerToPlant == null)
        {
            Debug.Log("Pot: No seeds available to plant.");
            return;
        }

        // consume seed (GameManager.UseSeed is authoritative)
        bool used = gm.UseSeed(flowerToPlant);
        if (!used)
        {
            Debug.LogWarning($"Pot: Failed to use seed for {flowerToPlant?.flowerName ?? "unknown"}.");
            return;
        }

        currentFlower = flowerToPlant;
        StartGrowthRoutine();
    }

    private void StartGrowthRoutine()
    {
        if (currentFlower == null)
        {
            Debug.LogError("Pot: currentFlower is null when starting growth. Aborting.");
            return;
        }

        if (growRoutine != null) StopCoroutine(growRoutine);
        growRoutine = StartCoroutine(GrowthCoroutine());
    }

    private IEnumerator GrowthCoroutine()
    {
        isGrowing = true;
        readyToHarvest = false;

        // defensive snapshot
        float elapsed = 0f;
        float growTime = (currentFlower != null) ? currentFlower.growTime : 0f;
        Sprite[] stages = (currentFlower != null) ? currentFlower.growthStages : null;

        // ensure starting sprite
        if (potImage != null && emptyPotSprite != null)
            potImage.sprite = emptyPotSprite;

        if (emptyToSproutTime > 0f)
            yield return new WaitForSeconds(emptyToSproutTime);

        // sprout stage
        float sproutTime = Mathf.Clamp01(sproutPercent) * growTime;
        float sproutElapsed = 0f;

        if (potImage != null)
        {
            if (sproutSprite != null)
                potImage.sprite = sproutSprite;
            else if (stages != null && stages.Length > 0 && stages[0] != null)
                potImage.sprite = stages[0];
            else if (emptyPotSprite != null)
                potImage.sprite = emptyPotSprite;
        }

        while (sproutElapsed < sproutTime)
        {
            sproutElapsed += Time.deltaTime;
            elapsed += Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.Ceil(Mathf.Max(0f, growTime - elapsed)) + "s";
            yield return null;
        }

        // remaining growth stages
        float remainingTime = Mathf.Max(0f, growTime - sproutTime);
        float stageElapsed = 0f;
        int numStages = stages != null ? stages.Length : 0;

        while (stageElapsed < remainingTime)
        {
            stageElapsed += Time.deltaTime;
            elapsed += Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.Ceil(Mathf.Max(0f, growTime - elapsed)) + "s";

            if (potImage != null && numStages > 1)
            {
                float stageProgress = remainingTime > 0f ? stageElapsed / remainingTime : 1f;
                int stageIndex = 1 + Mathf.FloorToInt(stageProgress * (numStages - 2));
                stageIndex = Mathf.Clamp(stageIndex, 1, numStages - 1);
                if (stages[stageIndex] != null)
                    potImage.sprite = stages[stageIndex];
            }

            yield return null;
        }

        // finished
        isGrowing = false;
        readyToHarvest = true;

        if (potImage != null && currentFlower != null && currentFlower.readySprite != null)
            potImage.sprite = currentFlower.readySprite;

        if (timerText != null) timerText.text = "!";
        growRoutine = null;
    }

    private void HarvestFlower()
    {
        if (!readyToHarvest && !isGrowing)
        {
            Debug.Log("Pot: Nothing to harvest.");
            return;
        }

        readyToHarvest = false;
        isGrowing = false;

        // Keep a temporary reference before resetting
        FlowerData harvestedFlower = currentFlower;
        currentFlower = null;

        // Reset visuals
        if (potImage != null && emptyPotSprite != null)
            potImage.sprite = emptyPotSprite;

        if (timerText != null)
            timerText.text = "";

        var gm = GameManager.Instance;
        if (gm != null && harvestedFlower != null)
        {
            gm.AddFlower(harvestedFlower, 1); // ✅ Add correct flower type
            gm.UpdateAllUI();                  // ✅ Ensure UI refresh after update
            Debug.Log($"Harvested {harvestedFlower.flowerName}, total now: {gm.GetFlowerCount(harvestedFlower)}");
        }
        else
        {
            Debug.LogWarning("Harvest failed: missing GameManager or harvestedFlower is null.");
        }

        SpawnHarvestPopup();

        Debug.Log($"Inventory now has {gm.GetFlowerInventory().Count} flower types tracked.");

    }


    private void SpawnHarvestPopup()
    {
        if (harvestPopupPrefab == null) return;

        // prefer explicitly assigned canvas
        Canvas canvas = targetCanvas != null ? targetCanvas : FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // fallback: instantiate in world space at spawn point
            Transform spawn = popupSpawnPoint != null ? popupSpawnPoint : transform;
            Instantiate(harvestPopupPrefab, spawn.position, Quaternion.identity);
            return;
        }

        // Instantiate under canvas so UI scale/sorting is correct.
        var popupGO = Instantiate(harvestPopupPrefab, canvas.transform, false);

        // If spawn point provided, convert its world position to canvas local point and set anchoredPosition
        if (popupSpawnPoint != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            RectTransform popupRect = popupGO.GetComponent<RectTransform>();
            if (canvasRect != null && popupRect != null)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, popupSpawnPoint.position);
                Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 localPoint))
                {
                    popupRect.anchoredPosition = localPoint;
                }
            }
        }
    }

    private void OnDisable()
    {
        // stop background coroutine to avoid leaks when object disabled
        if (growRoutine != null)
        {
            StopCoroutine(growRoutine);
            growRoutine = null;
        }
    }

    private void OnDestroy()
    {
        // ensure coroutine stopped on destroy as well
        if (growRoutine != null)
        {
            StopCoroutine(growRoutine);
            growRoutine = null;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Collider2D))]
public class Pot : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite emptyPotSprite;
    public Sprite sproutSprite; // optional

    [Header("Growth Timing")]
    public float sproutPercent = 0.2f; // percent of total time for sprout stage

    [Header("UI")]
    public TextMeshProUGUI timerText; // assign in inspector (can be null)

    [Header("Harvest popup")]
    public GameObject harvestPopupPrefab;
    public Transform popupSpawnPoint;
    public Canvas targetCanvas; // optional: where to parent popups

    // --- internals ---
    private UnityEngine.UI.Image uiImage;      // if pot is UI Image
    private SpriteRenderer spriteRenderer;    // if pot is world sprite

    // growth state
    private bool isGrowing = false;
    private bool readyToHarvest = false;
    private double plantedAtReal = 0.0;   // GlobalTime.RealNow when planted
    private double growDuration = 0.0;    // seconds total
    private FlowerData currentFlower;     // the FlowerData instance that this pot is growing

    private void Awake()
    {
        uiImage = GetComponentInChildren<UnityEngine.UI.Image>(); // for UI pot prefab
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set initial sprite
        if (uiImage != null && emptyPotSprite != null) uiImage.sprite = emptyPotSprite;
        if (spriteRenderer != null && emptyPotSprite != null) spriteRenderer.sprite = emptyPotSprite;

        if (timerText != null) timerText.text = "";
        // ensure collider exists (for clicks)
        if (GetComponent<Collider2D>() == null) gameObject.AddComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        // update visuals immediately when re-enabled (room re-entered)
        RefreshVisuals();
    }

    private void Update()
    {
        // If not growing, nothing to update (except maybe a little idle behavior)
        if (!isGrowing) return;

        // compute elapsed using GlobalTime where possible
        double now = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;
        double elapsed = now - plantedAtReal;

        // clamp elapsed
        if (elapsed < 0) elapsed = 0;

        // If elapsed >= growDuration -> finish
        if (elapsed >= growDuration)
        {
            FinishGrowth();
            return;
        }

        // Update timer text
        double remaining = growDuration - elapsed;
        if (timerText != null)
            timerText.text = Mathf.Ceil((float)remaining) + "s";

        // Update sprite stage based on progress
        UpdateSpriteByProgress((float)(elapsed / growDuration));
    }

    // Called when player plants a seed into this pot
    // (Call this from wherever you currently initiate StartGrowth)
    public void StartGrowth(FlowerData flower)
    {
        if (flower == null)
        {
            Debug.LogWarning("Pot.StartGrowth: flower null.");
            return;
        }

        // consume check should be done by caller (GameManager.UseSeed)
        currentFlower = flower;
        growDuration = Mathf.Max(0.1f, flower.growTime); // in seconds
        plantedAtReal = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;
        isGrowing = true;
        readyToHarvest = false;

        // immediate visual
        if (timerText != null) timerText.text = Mathf.Ceil((float)growDuration) + "s";
        if (sproutSprite != null) SetSprite(sproutSprite);
        else if (flower.growthStages != null && flower.growthStages.Length > 0 && flower.growthStages[0] != null)
            SetSprite(flower.growthStages[0]);
    }

    private void UpdateSpriteByProgress(float progress01)
    {
        if (currentFlower == null) return;
        Sprite[] stages = currentFlower.growthStages;
        if (stages == null || stages.Length == 0) return;

        // Sprout stage handled earlier; now map progress to stages[1..last]
        // We'll compute stageIndex from 0..stages.Length-1
        int totalStages = stages.Length;
        int stageIndex = Mathf.Clamp(Mathf.FloorToInt(progress01 * (totalStages - 1)), 0, totalStages - 1);
        // set sprite if valid
        if (stages[stageIndex] != null)
            SetSprite(stages[stageIndex]);
    }

    private void SetSprite(Sprite s)
    {
        if (uiImage != null) uiImage.sprite = s;
        if (spriteRenderer != null) spriteRenderer.sprite = s;
    }

    private void FinishGrowth()
    {
        isGrowing = false;
        readyToHarvest = true;

        if (currentFlower != null && currentFlower.readySprite != null)
            SetSprite(currentFlower.readySprite);
        else if (emptyPotSprite != null)
            SetSprite(emptyPotSprite);

        if (timerText != null) timerText.text = "!";
    }

    // Called by your harvest interaction (button/click)
    public void Harvest()
    {
        if (!readyToHarvest) return;

        readyToHarvest = false;
        isGrowing = false;

        // Reset sprite to empty
        if (emptyPotSprite != null) SetSprite(emptyPotSprite);
        if (timerText != null) timerText.text = "";

        // Add flower to GameManager (store per-flower in your GameManager.AddFlower overload)
        if (GameManager.Instance != null)
        {
            // ensure you have AddFlower(FlowerData, int) implemented
            GameManager.Instance.AddFlower(currentFlower, 1);
        }

        // spawn popup in canvas
        if (harvestPopupPrefab != null)
        {
            Canvas canvas = targetCanvas != null ? targetCanvas : FindObjectOfType<Canvas>();
            GameObject popup = Instantiate(harvestPopupPrefab);
            if (canvas != null)
                popup.transform.SetParent(canvas.transform, false);
        }

        currentFlower = null;
    }

    // Update visuals based on current state (call when entering room / enabling)
    private void RefreshVisuals()
    {
        if (isGrowing)
        {
            // force an immediate Update pass to refresh timer/sprite
            double now = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;
            double elapsed = now - plantedAtReal;
            if (elapsed < 0) elapsed = 0;
            if (elapsed >= growDuration)
            {
                FinishGrowth();
            }
            else
            {
                if (timerText != null) timerText.text = Mathf.Ceil((float)(growDuration - elapsed)) + "s";
                UpdateSpriteByProgress((float)(elapsed / growDuration));
            }
        }
        else
        {
            // Not growing
            if (emptyPotSprite != null) SetSprite(emptyPotSprite);
            if (timerText != null) timerText.text = "";
        }
    }

    // Optional helper if you need to serialize pot state / restore
    public PotState GetState()
    {
        return new PotState
        {
            isGrowing = this.isGrowing,
            readyToHarvest = this.readyToHarvest,
            plantedAtReal = this.plantedAtReal,
            growDuration = this.growDuration,
            flowerId = currentFlower != null ? currentFlower.name : null
        };
    }

    // Optional restore
    public void RestoreState(PotState state, Func<string, FlowerData> flowerLookup)
    {
        if (state == null) return;
        this.isGrowing = state.isGrowing;
        this.readyToHarvest = state.readyToHarvest;
        this.plantedAtReal = state.plantedAtReal;
        this.growDuration = state.growDuration;

        if (!string.IsNullOrEmpty(state.flowerId) && flowerLookup != null)
            this.currentFlower = flowerLookup(state.flowerId);

        RefreshVisuals();
    }

    [Serializable]
    public class PotState
    {
        public bool isGrowing;
        public bool readyToHarvest;
        public double plantedAtReal;
        public double growDuration;
        public string flowerId;
    }
}

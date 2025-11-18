using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;


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

    [Header("Flower Death Settings")]
    public float timeToDie = 30f; // seconds before flower dies if not harvested
    public Sprite wiltedSprite;   // optional sprite when flower dies

    [Header("Flower Death Visuals")]
    public Sprite[] dyingStages;  // Sprites to show as flower wilt

    private bool isDead = false;


    // internal tracking
    private double finishedAtReal = 0.0;


    // --- internals ---
    private Image uiImage;
    private SpriteRenderer spriteRenderer;

    public Button harvestButton; // assign in Inspector
    // growth state
    private bool isGrowing = false;
    private bool readyToHarvest = false;
    private double plantedAtReal = 0.0;
    private double growDuration = 0.0;
    private FlowerData currentFlower;

    private void Awake()
    {
        uiImage = GetComponentInChildren<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (uiImage != null && emptyPotSprite != null) uiImage.sprite = emptyPotSprite;
        if (spriteRenderer != null && emptyPotSprite != null) spriteRenderer.sprite = emptyPotSprite;

        if (timerText != null) timerText.text = "";
        if (GetComponent<Collider2D>() == null) gameObject.AddComponent<BoxCollider2D>();

        if (harvestButton != null)
        {
            harvestButton.onClick.RemoveAllListeners();
            harvestButton.onClick.AddListener(Harvest);
        }
    }

    private void OnEnable()
    {
        RefreshVisuals();
    }

    private void Die()
    {
        Debug.Log($"Flower in pot {gameObject.name} has fully died.");
        isGrowing = false;
        readyToHarvest = false;
        isDead = true;           // mark as dead
        currentFlower = null;

        SetSprite(wiltedSprite);
        if (timerText != null)
            timerText.text = "";
    }


    private void ResetPotAfterDeath()
    {
        Debug.Log($"Pot {gameObject.name} reset after dead flower.");
        isDead = false;
        isGrowing = false;
        readyToHarvest = false;
        currentFlower = null;

        // Reset visuals
        if (emptyPotSprite != null) SetSprite(emptyPotSprite);
        if (timerText != null) timerText.text = "";
    }



    private void Update()
    {
        if (!isGrowing && !readyToHarvest) return;

        double now = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;
        double elapsed = now - plantedAtReal;
        if (elapsed < 0) elapsed = 0;

        if (isGrowing)
        {
            if (elapsed >= growDuration)
            {
                FinishGrowth();
                return;
            }

            if (timerText != null)
                timerText.text = Mathf.Ceil((float)(growDuration - elapsed)) + "s";

            UpdateSpriteByProgress((float)(elapsed / growDuration));
        }
        else if (readyToHarvest)
        {
            double readyElapsed = now - finishedAtReal;

            // Update dying sprite based on progress
            if (dyingStages != null && dyingStages.Length > 0 && timeToDie > 0f)
            {
                float progress = Mathf.Clamp01((float)(readyElapsed / timeToDie));
                int stageIndex = Mathf.Min(Mathf.FloorToInt(progress * dyingStages.Length), dyingStages.Length - 1);
                if (dyingStages[stageIndex] != null)
                    SetSprite(dyingStages[stageIndex]);
            }

            // Optional: auto die at end
            if (readyElapsed >= timeToDie)
                Die();
        }

    }


    // Called from UI button or script when planting
    public void StartGrowth()
    {
        if (isDead)
        {
            ResetPotAfterDeath();
            return;
        }
        if (GameManager.Instance == null || GameManager.Instance.SelectedFlowerData == null)
        {
            Debug.LogWarning("Pot.StartGrowth: No flower selected.");
            return;
        }

        FlowerData flower = GameManager.Instance.SelectedFlowerData;

        // Consume seed
        if (!GameManager.Instance.UseSeed(flower))
        {
            Debug.Log("No seeds available for " + flower.name);
            return;
        }

        currentFlower = flower;
        growDuration = Mathf.Max(0.1f, flower.growTime);
        plantedAtReal = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;

        isGrowing = true;
        readyToHarvest = false;

        if (timerText != null)
            timerText.text = Mathf.Ceil((float)growDuration) + "s";

        if (sproutSprite != null)
            SetSprite(sproutSprite);
        else if (flower.growthStages != null && flower.growthStages.Length > 0 && flower.growthStages[0] != null)
            SetSprite(flower.growthStages[0]);
    }

    private void UpdateSpriteByProgress(float progress01)
    {
        if (currentFlower == null) return;
        Sprite[] stages = currentFlower.growthStages;
        if (stages == null || stages.Length == 0) return;

        int totalStages = stages.Length;
        int stageIndex = Mathf.Clamp(Mathf.FloorToInt(progress01 * (totalStages - 1)), 0, totalStages - 1);

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
        finishedAtReal = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;

        if (currentFlower != null && currentFlower.readySprite != null)
            SetSprite(currentFlower.readySprite);
        else if (emptyPotSprite != null)
            SetSprite(emptyPotSprite);

        if (timerText != null)
            timerText.text = "!";
    }



    // Harvest by clicking
    public void OnPointerClick(PointerEventData eventData)
    {
        if (readyToHarvest)
            Harvest();
        else if (isDead)
            ResetPotAfterDeath();
    }

    public void Harvest()
    {
        // Ensure pot is ready to harvest
        if (!readyToHarvest)
        {
            Debug.Log("Pot not ready to harvest yet.");
            return;
        }

        if (currentFlower == null)
        {
            Debug.LogWarning("Pot.Harvest: No flower data assigned.");
            return;
        }

        // Add flower to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFlower(currentFlower, 1);
            Debug.Log($"Harvested 1 {currentFlower.name} flower.");
        }

        // Spawn harvest popup
        if (harvestPopupPrefab != null)
        {
            Canvas canvas = targetCanvas != null ? targetCanvas : FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject popup = Instantiate(harvestPopupPrefab, canvas.transform);
                if (popupSpawnPoint != null)
                    popup.transform.position = popupSpawnPoint.position;
            }
        }

        // Reset pot state
        isGrowing = false;
        readyToHarvest = false;
        currentFlower = null;

        // Reset visuals
        if (emptyPotSprite != null) SetSprite(emptyPotSprite);
        if (timerText != null) timerText.text = "";
    }

 
    private void RefreshVisuals()
    {
        if (isGrowing)
        {
            double now = (GlobalTime.Instance != null) ? GlobalTime.Instance.RealNow : Time.realtimeSinceStartupAsDouble;
            double elapsed = now - plantedAtReal;
            if (elapsed >= growDuration)
                FinishGrowth();
            else
            {
                if (timerText != null)
                    timerText.text = Mathf.Ceil((float)(growDuration - elapsed)) + "s";
                UpdateSpriteByProgress((float)(elapsed / growDuration));
            }
        }
        else
        {
            if (emptyPotSprite != null) SetSprite(emptyPotSprite);
            if (timerText != null) timerText.text = "";
        }
    }
}

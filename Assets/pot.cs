using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Pot : MonoBehaviour
{
    [Header("Pot Sprites")]
    public Sprite emptyPotSprite;
    public Sprite sproutSprite;

    [Header("Growth Timing")]
    public float emptyToSproutTime = 0.5f;
    public float sproutPercent = 0.2f;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Harvest Popup")]
    public GameObject harvestPopupPrefab;
    public Transform popupSpawnPoint;

    private SpriteRenderer spriteRenderer;
    private Coroutine growRoutine;
    private bool isGrowing = false;
    private bool readyToHarvest = false;

    private FlowerData currentFlower;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = emptyPotSprite;
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

        // Determine which seed we can actually plant
        FlowerData flowerToPlant = gm.GetPlantableFlower(gm.selectedFlower);
        if (flowerToPlant == null)
        {
            Debug.Log("No seeds available to plant!");
            return;
        }

        // Consume seed
        gm.UseSeed(flowerToPlant);

        // Start growth
        currentFlower = flowerToPlant;
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
        float growTime = currentFlower.growTime;
        Sprite[] stages = currentFlower.growthStages;

        spriteRenderer.sprite = emptyPotSprite;
        yield return new WaitForSeconds(emptyToSproutTime);

        float sproutTime = growTime * sproutPercent;
        spriteRenderer.sprite = sproutSprite != null ? sproutSprite : stages[0];

        while (elapsed < growTime)
        {
            elapsed += Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.Ceil(growTime - elapsed) + "s";

            if (stages != null && stages.Length > 1)
            {
                float progress = elapsed / growTime;
                int stageIndex = Mathf.Clamp(Mathf.FloorToInt(progress * (stages.Length - 1)), 0, stages.Length - 1);
                spriteRenderer.sprite = stages[stageIndex];
            }

            yield return null;
        }

        isGrowing = false;
        readyToHarvest = true;
        spriteRenderer.sprite = currentFlower.readySprite;
        if (timerText != null) timerText.text = "!";
        growRoutine = null;
    }

    private void HarvestFlower()
    {
        readyToHarvest = false;
        spriteRenderer.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";

        GameManager.Instance.AddFlower(1);

        if (harvestPopupPrefab != null)
        {
            Transform spawn = popupSpawnPoint != null ? popupSpawnPoint : transform;
            GameObject popup = Instantiate(harvestPopupPrefab, spawn.position, Quaternion.identity);
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null) popup.transform.SetParent(canvas.transform, false);
        }
    }
}

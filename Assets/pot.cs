using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class Pot : MonoBehaviour
{
    [Tooltip("Optional unique ID for this pot. If empty, it will be auto-generated using scene name + position.")]
    public string potID;

    [Header("Growth settings")]
    public float growTime = 20f;
    public Sprite emptyPotSprite;
    public Sprite[] growthStages; // ordered from earliest → latest

    [Header("Timer UI")]
    public TextMeshProUGUI timerText; // child UI above the pot

    private SpriteRenderer spriteRenderer;
    private bool isGrowing = false;
    private Coroutine growCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";
        // Auto-generate ID if none provided (rounded position to reduce float noise)
        if (string.IsNullOrEmpty(potID))
        {
            Vector3 p = transform.position;
            float rx = Mathf.Round(p.x * 100f) / 100f;
            float ry = Mathf.Round(p.y * 100f) / 100f;
            potID = SceneManager.GetActiveScene().name + $"_{rx}_{ry}";
        }
    }

    private void Start()
    {
        // If this pot already has a state in the GameManager (planted previously),
        // resume the visual timer/sprites from that state.
        if (GameManager.Instance != null && GameManager.Instance.TryGetPotState(potID, out PotState ps))
        {
            float remaining = ps.endTime - Time.realtimeSinceStartup;
            if (remaining > 0f)
            {
                // resume growth from remaining time
                StartGrowthRoutine();
            }
            else
            {
                // It finished while we were away — the GameManager's Update() should have
                // already processed it, but ensure visuals are reset:
                spriteRenderer.sprite = emptyPotSprite;
                if (timerText != null) timerText.text = "";
            }
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance == null) return;
        if (isGrowing) return;
        if (GameManager.Instance.seedCount <= 0)
        {
            Debug.Log("No seeds available to plant.");
            return;
        }

        // spend one seed
        GameManager.Instance.AddSeed(-1);

        // Start the authoritative pot growth in GameManager (endTime = now + growTime)
        GameManager.Instance.StartPotGrowth(potID, growTime);

        // Start local visual routine
        StartGrowthRoutine();
    }

    private void StartGrowthRoutine()
    {
        if (growCoroutine != null) StopCoroutine(growCoroutine);
        growCoroutine = StartCoroutine(GrowthCoroutine());
    }

    private IEnumerator GrowthCoroutine()
    {
        isGrowing = true;

        while (true)
        {
            // Get latest state from GameManager (authoritative)
            if (GameManager.Instance == null || !GameManager.Instance.TryGetPotState(potID, out PotState ps))
            {
                // no state => finished or cleared
                break;
            }

            float remaining = ps.endTime - Time.realtimeSinceStartup;

            if (remaining <= 0f)
            {
                // Growth finished — exit loop and let GameManager process completion
                break;
            }

            // update timer text
            if (timerText != null) timerText.text = Mathf.Ceil(remaining).ToString() + "s";

            // update sprite based on progress
            float progress = 1f - Mathf.Clamp01(remaining / ps.growTime); // 0 → 1
            if (growthStages != null && growthStages.Length > 0)
            {
                int stageIndex = Mathf.FloorToInt(progress * growthStages.Length);
                stageIndex = Mathf.Clamp(stageIndex, 0, growthStages.Length - 1);
                spriteRenderer.sprite = growthStages[stageIndex];
            }

            yield return null; // wait one frame
        }

        // Try to complete via GameManager (idempotent). If it returns true, it added a flower and removed state.
        if (GameManager.Instance != null)
        {
            bool completedNow = GameManager.Instance.TryCompletePot(potID);
            if (!completedNow)
            {
                // If TryCompletePot returned false, either GameManager already cleared it and added the flower,
                // or there was a slight timing issue — to be safe, ensure visuals reset:
                GameManager.Instance.ClearPotState(potID);
            }
        }

        // Reset visuals
        spriteRenderer.sprite = emptyPotSprite;
        if (timerText != null) timerText.text = "";
        isGrowing = false;
        growCoroutine = null;
    }
}

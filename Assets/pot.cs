using UnityEngine;
using TMPro;
using System.Collections;

public class Pot : MonoBehaviour
{
    [Header("Plant Growth Settings")]
    public float growTime = 20f; // seconds to grow
    public Sprite emptyPotSprite;
    public GameObject sproutPrefab;      // Prefab for the sprout
    public GameObject grownPlantPrefab;  // Prefab for the grown plant

    [Header("UI")]
    public TextMeshProUGUI timerText; // TMP text above the pot

    private SpriteRenderer spriteRenderer;
    private bool isGrowing = false;
    private float timer = 0f;
    private GameObject currentSprout;
    private GameObject currentGrownPlant;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogWarning("SpriteRenderer component missing on Pot.");
        if (timerText != null)
            timerText.text = ""; // hide timer at start
        else
            Debug.LogWarning("TimerText UI reference is missing on Pot.");
        if (emptyPotSprite != null)
            spriteRenderer.sprite = emptyPotSprite;
    }

    private void OnMouseDown()
    {
        if (isGrowing)
            return;

        if (currentGrownPlant != null)
        {
            // Harvest the grown plant
            Destroy(currentGrownPlant);
            currentGrownPlant = null;
            if (spriteRenderer != null && emptyPotSprite != null)
                spriteRenderer.sprite = emptyPotSprite;
            // TODO: Add reward for harvesting here (e.g., GameManager.Instance.AddMoney(...))
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing.");
            return;
        }

        // Only plant if you have seeds and the pot is empty
        if (GameManager.Instance.seedCount > 0)
        {
            if (GameManager.Instance.SpendSeed(1))
            {
                StartCoroutine(GrowPlant());
            }
            else
            {
                Debug.Log("Not enough seeds to plant.");
            }
        }
        else
        {
            Debug.Log("Not enough seeds to plant.");
        }
    }

    /// <summary>
    /// Coroutine to handle plant growth over time.
    /// </summary>
    IEnumerator GrowPlant()
    {
        isGrowing = true;

        // Instantiate sprout inside the pot
        if (sproutPrefab != null)
        {
            currentSprout = Instantiate(sproutPrefab, transform);
            currentSprout.transform.localPosition = Vector3.zero; // Center sprout in pot
        }
        else
        {
            Debug.LogWarning("Sprout prefab is not assigned.");
        }

        timer = growTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timerText != null)
                timerText.text = $"{Mathf.Ceil(timer)}s";
            yield return null;
        }

        // Plant finished: remove sprout and spawn grown plant
        if (timerText != null)
            timerText.text = "";

        if (currentSprout != null)
            Destroy(currentSprout);

        if (grownPlantPrefab != null)
        {
            currentGrownPlant = Instantiate(grownPlantPrefab, transform);
            currentGrownPlant.transform.localPosition = Vector3.zero; // Center grown plant in pot
        }
        else
        {
            Debug.LogWarning("Grown plant prefab is not assigned.");
        }

        isGrowing = false;

        // Now the pot can be harvested and reused
    }
}

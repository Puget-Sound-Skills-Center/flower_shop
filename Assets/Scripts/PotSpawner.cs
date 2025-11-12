using UnityEngine;

public class PotSpawner : MonoBehaviour
{
    [Header("Growing Area")]
    public Transform spawnParent;           // Parent transform for pots in the backroom
    public GameObject growingPotPrefab;     // Prefab with the Pot.cs script

    /// <summary>
    /// Spawn a new pot in the backroom at a specific local position
    /// </summary>
    public void SpawnPot(Vector3 localPosition)
    {
        if (growingPotPrefab == null || spawnParent == null)
        {
            Debug.LogWarning("PotSpawner: Prefab or parent not assigned!");
            return;
        }

        GameObject newPot = Instantiate(growingPotPrefab, spawnParent);
        newPot.transform.localPosition = localPosition;
        newPot.transform.localRotation = Quaternion.identity;
        newPot.transform.localScale = Vector3.one;

        // Initialize Pot.cs if needed
        var potComponent = newPot.GetComponent<Pot>();
        if (potComponent != null)
        {
            // Currently Pot.cs doesn't have a public Initialize, but it sets up empty pot in Awake
            // Optional: you could add a public method in Pot.cs like `ResetPot()` if needed
        }
    }

    /// <summary>
    /// Spawn multiple pots in a simple row or grid
    /// </summary>
    public void SpawnMultiplePots(int count, float spacing = 1.5f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3((i % 5) * spacing, 0, (i / 5) * spacing); // row of 5 per line
            SpawnPot(pos);
        }
    }
}

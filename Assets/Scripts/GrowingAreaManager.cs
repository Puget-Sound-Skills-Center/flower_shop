using UnityEngine;

public class GrowingAreaManager : MonoBehaviour
{
    [Header("Parent & Prefab")]
    public Transform potGridParent;       // Empty GameObject with GridLayoutGroup
    public int maxPots = 20;             // Maximum pots allowed in the growing area

    /// <summary>
    /// Spawn a number of pots using the specified prefab
    /// </summary>
    public void AddPots(int count, GameObject prefab)
    {
        if (prefab == null || potGridParent == null)
        {
            Debug.LogWarning("GrowingAreaManager: Missing prefab or parent reference.");
            return;
        }

        int currentCount = GetCurrentPotCount();
        int availableSpace = maxPots - currentCount;

        if (availableSpace <= 0)
        {
            Debug.Log("GrowingAreaManager: Max capacity reached. Cannot add more pots.");
            return;
        }

        int spawnCount = Mathf.Min(count, availableSpace);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newPot = Instantiate(prefab, potGridParent);
            newPot.transform.localPosition = Vector3.zero; // GridLayoutGroup handles positioning
            newPot.transform.localRotation = Quaternion.identity;
            newPot.transform.localScale = Vector3.one;
        }

        if (spawnCount < count)
            Debug.Log("Reached max capacity, some pots were not spawned.");
    }

    /// <summary>
    /// Get the current number of pots in the growing area
    /// </summary>
    public int GetCurrentPotCount()
    {
        return potGridParent != null ? potGridParent.childCount : 0;
    }

    /// <summary>
    /// Clear all pots from the growing area
    /// </summary>
    public void ClearPots()
    {
        foreach (Transform child in potGridParent)
            Destroy(child.gameObject);
    }
}

using UnityEngine;

public class GrowingAreaManager : MonoBehaviour
{
    [Header("Parent & Prefab")]
    public GameObject potPrefab;      // Prefab to spawn in the growing area
    public Transform potGridParent;   // Empty GameObject with GridLayoutGroup

    /// <summary>
    /// Spawn a number of pots into the growing area.
    /// </summary>
    /// <param name="count">Number of pots to spawn</param>
    public void AddPots(int count)
    {
        if (potPrefab == null || potGridParent == null)
        {
            Debug.LogWarning("GrowingAreaManager: Missing prefab or parent reference.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject newPot = Instantiate(potPrefab, potGridParent);
            newPot.transform.localPosition = Vector3.zero; // GridLayout handles positioning
            newPot.transform.localRotation = Quaternion.identity;
            newPot.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Optionally clear all pots from the grid
    /// </summary>
    public void ClearPots()
    {
        foreach (Transform child in potGridParent)
        {
            Destroy(child.gameObject);
        }
    }
}

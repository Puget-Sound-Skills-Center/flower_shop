using UnityEngine;

public class GardenRoomManager : MonoBehaviour
{
    public Transform decorationParent; // Empty GameObject in GardenRoom

    private void OnEnable()
    {
        foreach (var deco in GameManager.Instance.gardenDecorations)
        {
            if (deco.unlocked && deco.decorationPrefab != null)
            {
                // Instantiate only if not already present
                if (decorationParent.Find(deco.decorationPrefab.name) == null)
                {
                    GameObject newDeco = Instantiate(deco.decorationPrefab, decorationParent);
                    newDeco.name = deco.decorationPrefab.name; // prevent duplicates
                }
            }
        }
    }
}

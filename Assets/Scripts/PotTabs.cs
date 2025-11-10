using UnityEngine;
using System.Collections.Generic;

public class PotsTab : MonoBehaviour
{
    [Header("References")]
    public Transform potGridParent; // Parent containing PotShopItem prefabs

    [Header("Bundle Settings (Inspector)")]
    public PotBundleData[] potBundles;

    private List<PotShopItem> potItems = new List<PotShopItem>();

    private void OnEnable()
    {
        InitializePotItems();
    }

    private void InitializePotItems()
    {
        potItems.Clear();

        if (potGridParent == null)
        {
            Debug.LogWarning("PotsTab: potGridParent not assigned.");
            return;
        }

        int childIndex = 0;
        foreach (var bundle in potBundles)
        {
            if (childIndex >= potGridParent.childCount)
            {
                Debug.LogWarning("Not enough PotShopItem prefabs in grid for bundles.");
                break;
            }

            var item = potGridParent.GetChild(childIndex).GetComponent<PotShopItem>();
            if (item != null)
            {
                item.potName = bundle.potName;
                item.bundleAmount = bundle.bundleAmount;
                item.cost = bundle.cost;
                item.owned = 0; // start with zero

                item.UpdateUI();
                potItems.Add(item);
            }

            childIndex++;
        }
    }
}

[System.Serializable]
public struct PotBundleData
{
    public string potName;
    public int bundleAmount;
    public int cost;
}

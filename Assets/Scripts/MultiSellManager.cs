using UnityEngine;
using System.Collections.Generic;

public class MultiSellManager : MonoBehaviour
{
    public static MultiSellManager Instance;

    private List<ShelfBouquetButton> selectedBouquets = new List<ShelfBouquetButton>();

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateSelection(ShelfBouquetButton bouquet)
    {
        if (bouquet.isSelected)
            selectedBouquets.Add(bouquet);
        else
            selectedBouquets.Remove(bouquet);
    }

    public void SellSelectedBouquets()
    {
        if (selectedBouquets.Count == 0)
        {
            SellPopupManager.Instance.ShowPopup("No bouquets selected!", Input.mousePosition);
            return;
        }

        int totalEarned = 0;

        foreach (var b in selectedBouquets)
        {
            GameManager.Instance.SellBouquet(b.flowerData, b.flowerData.bouquetSellPrice);
            totalEarned += b.flowerData.bouquetSellPrice;
            Destroy(b.gameObject);
        }

        selectedBouquets.Clear();

        SellPopupManager.Instance.ShowPopup(
            $"+${totalEarned} (bouquets sold)",
            Input.mousePosition
        );
    }
}

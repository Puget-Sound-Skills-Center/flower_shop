using UnityEngine;
using UnityEngine.UI;

public class ShelfBouquetButton : MonoBehaviour
{
    public FlowerData flowerData;
    public ShopShelf shopShelf;
    public int sellPrice = 30;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClickSell);
    }

    private void OnClickSell()
    {
        GameManager.Instance.SellBouquetFromButton(this);
    }

    public FlowerData GetFlowerData() => flowerData;
}


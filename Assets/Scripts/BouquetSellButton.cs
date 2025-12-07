using UnityEngine;
using UnityEngine.UI;

public class BouquetSellButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SellBouquets);
    }

    private void SellBouquets()
    {
        MultiSellManager.Instance.SellSelectedBouquets();
    }
}

using UnityEngine;
using TMPro;

public class UIInitializer : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;

    private void Start()
    {
        // If GameManager exists, link new UI
        if (GameManager.Instance != null)
        {
            GameManager.Instance.moneyText = moneyText;
            GameManager.Instance.seedText = seedText;
            GameManager.Instance.flowerText = flowerText;
            GameManager.Instance.UpdateAllUI();
        }
    }
}

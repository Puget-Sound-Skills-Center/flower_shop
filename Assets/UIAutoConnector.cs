using UnityEngine;
using TMPro;

public class UIAutoConnector : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // Look for UI elements by name in the current scene
            TextMeshProUGUI moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI seedText = GameObject.Find("SeedText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI flowerText = GameObject.Find("FlowerText")?.GetComponent<TextMeshProUGUI>();

            // Assign them to GameManager if they exist
            if (moneyText != null) GameManager.Instance.moneyText = moneyText;
            if (seedText != null) GameManager.Instance.seedText = seedText;
            if (flowerText != null) GameManager.Instance.flowerText = flowerText;

            // Refresh the UI
            GameManager.Instance.UpdateAllUI();
        }
    }
}

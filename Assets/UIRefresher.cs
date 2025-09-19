using UnityEngine;
using TMPro;

public class UIRefresher : MonoBehaviour
{
    [Header("Assign the TMP text objects for this room")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI flowerText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            // Re-bind UI references so GameManager points to the active room's UI
            if (moneyText != null) GameManager.Instance.moneyText = moneyText;
            if (seedText != null) GameManager.Instance.seedText = seedText;
            if (flowerText != null) GameManager.Instance.flowerText = flowerText;

            // Force UI update
            GameManager.Instance.UpdateAllUI();
        }
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Shop : MonoBehaviour
{
    public Button sellButton;
    public TextMeshProUGUI sellResultText;
    public int sellPricePerFlower = 5;

    private void Start()
    {
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellFlowers);

        if (sellResultText == null)
            Debug.LogWarning("Shop: sellResultText reference is missing.");
    }

    private void OnSellFlowers()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("Shop: GameManager instance missing.");
            return;
        }

        int totalFlowers = 0;
        int totalEarned = 0;

        foreach (var kvp in gm.GetFlowerInventory())
        {
            totalFlowers += kvp.Value;
            totalEarned += kvp.Value * sellPricePerFlower;
            gm.AddFlower(kvp.Key, -kvp.Value); // remove flowers
        }

        if (totalFlowers > 0)
        {
            gm.AddMoney(totalEarned);
            if (sellResultText != null)
                sellResultText.text = $"Sold {totalFlowers} flowers for ${totalEarned}!";
        }
        else
        {
            if (sellResultText != null)
                sellResultText.text = "No flowers to sell!";
        }

        StartCoroutine(ResetResultText());
    }

    private IEnumerator ResetResultText()
    {
        yield return new WaitForSeconds(1f);
        if (sellResultText != null)
            sellResultText.text = "";
    }
}

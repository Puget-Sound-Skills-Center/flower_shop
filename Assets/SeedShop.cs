using UnityEngine;

public class SeedShop : MonoBehaviour
{
    public GameObject seedPrefab;
    public Canvas canvas;
    public int seedCost = 2;

    public void BuySeed()
    {
        if (GameManager.Instance.SpendMoney(seedCost))
        {
            // Spawn inside the canvas
            GameObject seed = Instantiate(seedPrefab, canvas.transform);

            // Reset position to the center of the screen
            RectTransform rectTransform = seed.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.Log("Not enough money to buy a seed!");
        }
    }
}

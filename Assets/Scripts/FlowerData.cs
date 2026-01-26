using UnityEngine;

[CreateAssetMenu(fileName = "NewFlower", menuName = "Flower Shop/Flower")]
public class FlowerData : ScriptableObject
{
    [Header("General Flower Information")]
    public string flowerName;
    [Header("Tooltip / Info Panel")]
    public GameObject tooltipPrefab;
    [TextArea(3, 6)]
    public string tooltipDescription;

    [Header("Growth Sprites")]
    public Sprite[] growthStages;     // Sprites used while growing in a pot
    public Sprite readySprite;        // Final fully grown sprite

    [Header("Bouquet Desk Sprites")]
    public Sprite bouquetDefaultSprite;   // When first selected
    public Sprite bouquetCutSprite;       // Cut stage
    public Sprite bouquetWrappedSprite;   // Wrapped stage
    public Sprite bouquetRibbonSprite;    // Ribbon stage
    public Sprite bouquetFinalSprite;     // Final bouquet to place on shelf

    [Header("Death Sprites")]
    public Sprite wiltedSprite;      // Optional per-flower wilted sprite (preferred over pot-level)

    [Header("Flower Stats")]
    public float growTime = 20f;
    public int seedCost = 2;
    internal int price;
    public int bouquetSellPrice = 20;  // default value
}

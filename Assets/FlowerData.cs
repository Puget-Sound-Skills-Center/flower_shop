using UnityEngine;

[CreateAssetMenu(fileName = "NewFlower", menuName = "Flower Shop/Flower")]
public class FlowerData : ScriptableObject
{
    public string flowerName;
    public Sprite[] growthStages;    // Sprites for growth stages
    public Sprite readySprite;       // Final fully grown sprite
    public float growTime = 20f;     // Growth duration in seconds
    public int sellValue = 5;        // Value when sold
}

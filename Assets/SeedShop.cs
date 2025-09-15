using UnityEngine;
using UnityEngine.UI;

public class SeedShop : MonoBehaviour
{
    public GameObject seedPrefab;  // Draggable seed prefab
    public Transform seedSpawnArea; // Where new seeds spawn (UI/scene)

    public void BuySeed()
    {
        GameObject seed = Instantiate(seedPrefab, seedSpawnArea);
        seed.transform.localPosition = Vector3.zero; // spawn centered in spawn area
    }
}

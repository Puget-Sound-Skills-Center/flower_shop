using UnityEngine;
using UnityEngine.EventSystems;

public class Pot : MonoBehaviour, IDropHandler
{
    public GameObject plantPrefab;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableSeed seed = eventData.pointerDrag.GetComponent<DraggableSeed>();
        if (seed != null)
        {
            // Spawn plant
            Instantiate(plantPrefab, transform.position, Quaternion.identity, transform);

            // Remove seed
            Destroy(seed.gameObject);
        }
    }
}

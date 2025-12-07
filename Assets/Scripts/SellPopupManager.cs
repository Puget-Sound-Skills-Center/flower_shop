using UnityEngine;
using TMPro;

public class SellPopupManager : MonoBehaviour
{
    public static SellPopupManager Instance;

    [Header("Popup Prefab")]
    public GameObject popupPrefab; // Assign your popup prefab in Inspector
    public Canvas canvas;          // Main UI canvas

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message, Vector3 worldPosition)
    {
        if (popupPrefab == null || canvas == null)
        {
            Debug.LogError("SellPopupManager: Missing popupPrefab or canvas!");
            return;
        }

        GameObject popupObj = Instantiate(popupPrefab, canvas.transform);
        popupObj.transform.position = worldPosition;

        var text = popupObj.GetComponent<TextMeshProUGUI>();
        text.text = message;
    }
}

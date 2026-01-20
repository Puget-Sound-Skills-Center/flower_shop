using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Prefab")]
    [SerializeField] private GameObject settingsPanelPrefab;

    private GameObject settingsInstance;
    private Canvas currentCanvas;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvasAndAttach();
    }

    private void FindCanvasAndAttach()
    {
        currentCanvas = FindObjectOfType<Canvas>();

        if (currentCanvas == null)
        {
            Debug.LogWarning("SettingsManager: No Canvas found in scene.");
            return;
        }

        if (settingsInstance == null)
        {
            settingsInstance = Instantiate(settingsPanelPrefab, currentCanvas.transform);
            settingsInstance.SetActive(false);
        }
        else
        {
            settingsInstance.transform.SetParent(currentCanvas.transform, false);
        }
    }

    public void Open()
    {
        if (settingsInstance == null) return;
        settingsInstance.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Close()
    {
        if (settingsInstance == null) return;
        settingsInstance.SetActive(false);
        Time.timeScale = 1f;
    }

    public bool IsOpen()
    {
        return settingsInstance != null && settingsInstance.activeSelf;
    }
}

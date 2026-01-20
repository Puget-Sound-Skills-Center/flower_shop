using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;

    [Header("Gameplay UI (DISABLED ON PAUSE)")]
    [SerializeField] private Canvas[] gameplayCanvases;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private bool isPaused;
    private GraphicRaycaster pauseRaycaster;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pauseRaycaster = pauseCanvas.GetComponent<GraphicRaycaster>();
        pausePanel.SetActive(false);
        pauseRaycaster.enabled = false;

        Time.timeScale = 1f;
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause();

        if (isPaused && SettingsManager.Instance.IsOpen())
        {
            return;
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        // 🔒 Disable gameplay UI interaction
        foreach (var canvas in gameplayCanvases)
        {
            var ray = canvas.GetComponent<GraphicRaycaster>();
            if (ray) ray.enabled = false;
        }

        // ✅ Enable pause UI only
        pausePanel.SetActive(true);
        pauseRaycaster.enabled = true;

        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // 🔓 Re-enable gameplay UI
        foreach (var canvas in gameplayCanvases)
        {
            var ray = canvas.GetComponent<GraphicRaycaster>();
            if (ray) ray.enabled = true;
        }

        pauseRaycaster.enabled = false;
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void OpenSettings()
    {
        SettingsManager.Instance.Open();
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
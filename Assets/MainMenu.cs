using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
      
        // Hide settings at start
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Populate resolutions dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);


        // Set default volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("FlowerShop");
    }

    public void OpenOptions()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, res.refreshRate);
        Debug.Log("Setting resolution to: " + res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

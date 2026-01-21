using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    Resolution[] reolutions;

    private void Start()
    {
       resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

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
    }

    public void PlayGame()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadScene("FlowerShop");
    }

    public void OpenOptions()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }


    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}

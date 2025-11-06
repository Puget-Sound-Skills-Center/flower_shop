using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private void Awake()
    {
        // Singleton pattern to persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved volume if available
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioListener.volume = masterVolume;
        }
    }

    public void SetVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public float GetVolume()
    {
        return masterVolume;
    }
}

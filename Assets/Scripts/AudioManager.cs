using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Sound Library")]
    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volumes
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumes();

        // Build sound dictionary
        foreach (Sound s in sounds)
        {
            soundDict[s.name] = s;
        }
    }

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if (musicSource != null) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        if (sfxSource != null) sfxSource.volume = volume;
    }

    public void PlayMusic(string name)
    {
        if (soundDict.TryGetValue(name, out Sound s))
        {
            musicSource.clip = s.clip;
            musicSource.loop = s.loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music not found: " + name);
        }
    }

    public void PlaySFX(string name)
    {
        if (soundDict.TryGetValue(name, out Sound s))
        {
            sfxSource.PlayOneShot(s.clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + name);
        }
    }
}

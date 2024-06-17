using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void ChangeMusicVolume(float value)
    {
        MusicVolume = value;
        ApplyMusicVolume();
    }

    public void ChangeSFXVolume(float value)
    {
        SFXVolume = value;
        ApplySFXVolume();
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void ApplyMusicVolume()
    {
        AudioSource[] musicSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource musicSource in musicSources)
        {
            if (musicSource.CompareTag("Music"))
            {
                musicSource.volume = MusicVolume;
            }
        }
    }

    private void ApplySFXVolume()
    {
        AudioSource[] sfxSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource sfxSource in sfxSources)
        {
            if (sfxSource.CompareTag("Player") || sfxSource.CompareTag("Enemy"))
            {
                sfxSource.volume = SFXVolume;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicVolume();
        ApplySFXVolume();
    }
}
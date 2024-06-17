using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern para asegurar que solo haya una instancia de GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    // Metodo para iniciar el juego
    public void StartGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
    
    public void ChangeMusicVolume(float value)
    {
        // Asumiendo que hay un solo AudioSource para la música
        //AudioSource musicSource = FindObjectOfType<AudioSource>();
        /*if (musicSource != null)
        {
            musicSource.volume = value;
        }
        else
        {
            Debug.LogWarning("No AudioSource found for music.");
        }*/
        AudioSource[] music = FindObjectsOfType<AudioSource>();
        foreach (AudioSource musicSource in music)
        {
            if (musicSource.tag == "Music")
            {
                musicSource.volume = value;
            }
            else
            {
                Debug.LogWarning("No AudioSource found for music.");
            }
        }
    }
    
    public void ChangeSFXVolume(float value)
    {
        AudioSource[] sfxSources = FindObjectsOfType<AudioSource>(); 
        foreach (AudioSource sfxSource in sfxSources)
        {
            if (sfxSource.tag == "Player" || sfxSource.tag == "Enemy" )
            {
                sfxSource.volume = value;
            }
        }
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }
}
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
            DontDestroyOnLoad(gameObject); // Previene que el objeto GameManager se destruya al cargar una nueva escena
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

    // Metodo para cambiar el volumen de la música
    public void ChangeMusicVolume(float value)
    {
        // Asumiendo que hay un solo AudioSource para la música
        AudioSource musicSource = FindObjectOfType<AudioSource>(); // Asumiendo que hay un solo AudioSource para la música
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
        else
        {
            Debug.LogWarning("No AudioSource found for music.");
        }
    }

    // Metodo para cambiar el volumen de los SFX
    public void ChangeSFXVolume(float value)
    {
        // Asumiendo que los SFX tienen un tag "SFX", se puede ajustar el volumen de todos los AudioSources con ese tag
        AudioSource[] sfxSources = FindObjectsOfType<AudioSource>(); // Find all AudioSources
        foreach (AudioSource sfxSource in sfxSources)
        {
            if (sfxSource.tag == "SFX") // Assumiendo que los SFX tienen un tag "SFX"
            {
                sfxSource.volume = value;
            }
        }
    }

    // Metodo para cambiar los volumenes
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }
}
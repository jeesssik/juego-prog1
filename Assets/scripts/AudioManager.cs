using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource ambientSound;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        CheckSceneAndPlayAudio(SceneManager.GetActiveScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneAndPlayAudio(scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        CheckSceneAndPlayAudio(SceneManager.GetActiveScene());
    }

    private void CheckSceneAndPlayAudio(Scene scene)
    {
        if (scene.name == "Level1")
        {
            PlayAudio();
        }
        else
        {
            StopAudio();
        }
    }

    private void PlayAudio()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }

        if (ambientSound != null && !ambientSound.isPlaying)
        {
            ambientSound.Play();
        }
    }

    private void StopAudio()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        if (ambientSound != null && ambientSound.isPlaying)
        {
            ambientSound.Stop();
        }
    }
}


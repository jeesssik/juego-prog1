using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanelUI;
    public Button resumeButton;
    public Button settingsButton;
    public Button exitButton;
    public Button backButton; 
    public Slider volumeSlider; 
    public Slider musicSlider; 
    public Slider sfxSlider; 

    private bool isPaused = false;

    void Start()
    {
        // Inicializar botones
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
        backButton.onClick.AddListener(CloseSettings);

        // Inicializar sliders
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);

        // Ocultar el menú de pausa y el panel de configuración al inicio
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
        Time.timeScale = 1f; // Reanudar el tiempo
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsPanelUI.SetActive(false);
        Time.timeScale = 0f; // Pausar el tiempo
        isPaused = true;
    }

   
    void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(true);
    }

    void CloseSettings()
    {
        pauseMenuUI.SetActive(true);
        settingsPanelUI.SetActive(false);
    }

    void ChangeMusicVolume(float value)
    {
        
        Debug.Log("Cambiar volumen de la música a: " + value);
        GameManager.Instance.ChangeMusicVolume(value);
    }

    void ChangeSFXVolume(float value)
    {
        Debug.Log("Cambiar volumen de los efectos de sonido a: " + value);
        GameManager.Instance.ChangeSFXVolume(value);
    }

    void ChangeVolume(float value)
    {
        
        Debug.Log("Cambiar volumen general a: " + value);
        GameManager.Instance.ChangeVolume(value);
    }
    void ExitGame()
    {
        Application.Quit();
    }
}

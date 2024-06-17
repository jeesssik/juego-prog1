using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UImenuPrincipal : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    
    //[SerializeField] private RectTransform buttonContainer;

    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject settingsScreen;
    
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;


    private void Awake()
    {
        
        playButton.onClick.AddListener(Play);
        settingsButton.onClick.AddListener(Settings);
        exitButton.onClick.AddListener(Exit);
        backButton.onClick.AddListener(GoBack);
        
        sliderMusic.onValueChanged.AddListener(ChangeMusicVolume);
        sliderSFX.onValueChanged.AddListener(ChangeSFXVolume);
        sliderVolume.onValueChanged.AddListener(ChangeVolume);
        
        GoBack();
    }


    private void Play()
    {
        SceneManager.LoadScene("Level1");
    }
    
    private void Settings()
    {
        mainScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    private void GoBack()
    {
        mainScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }
    
    private void Exit()
    {
        Application.Quit();
    }
    
    private void ChangeMusicVolume(float value)
    {
       GameManager.Instance.ChangeMusicVolume(value);
    }
    
    private void ChangeSFXVolume(float value)
    {
      GameManager.Instance.ChangeSFXVolume(value);
    }
    
    private void ChangeVolume(float value)
    {
        GameManager.Instance.ChangeVolume(value);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    public Button mainScreenButton;
    void Start()
    {
        mainScreenButton.onClick.AddListener(LoadMainScreen);
    }
    
    public void LoadMainScreen()
    {
        SceneManager.LoadScene("MainScreen");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startSceneName = "IntroScene";
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        try
        {
            SceneManager.LoadScene(startSceneName);
            DataPersistentManager.instance.NewGame();
        }
        catch
        {
            Debug.LogError(@"MainMenu: Scene not found: " + startSceneName);
        }
    }

    public void MuteHandler(bool mute)
    {
        if (mute)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
	    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

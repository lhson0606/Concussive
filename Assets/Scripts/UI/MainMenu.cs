using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public string startSceneName = "IntroScene";
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject continueMenu;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (DataPersistentManager.instance.hasGameData())
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }
    public void PlayGame()
    {
        try
        {
            DataPersistentManager.instance.NewGame();
            DataPersistentManager.instance.SaveGame();
            SceneManager.LoadScene(startSceneName);
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

    public void OpenContinueMenu()
    {
        mainMenu.SetActive(false);
        continueMenu.SetActive(true);
    }

    public void CloseContinueMenu()
    {
        continueMenu.SetActive(false);
        mainMenu.SetActive(true);
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

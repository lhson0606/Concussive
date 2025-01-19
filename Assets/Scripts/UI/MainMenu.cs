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
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle soundToggle;

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

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void ToggleSoundClick()
    {
        if (soundToggle.isOn)
        {
            AudioListener.volume = 0.5f;
        }
        else
        {
            AudioListener.volume = 0;
        }
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
	
    public void QuitGame()
    {
        // Debug.Log("Quit Game");
        Application.Quit();
    }
}

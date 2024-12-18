using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void Pause()
    {
        Time.timeScale = 0f; // Pause the game
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f; // Resume the game
        pauseMenu.SetActive(false);
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu"); // Load the home screen scene
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Ensure the game is not paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}

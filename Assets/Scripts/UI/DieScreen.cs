using UnityEngine;
using UnityEngine.SceneManagement;

public class DieScreen : MonoBehaviour
{
    [SerializeField] private GameObject dieMenu;

    public void ShowDieScreen()
    {
        Time.timeScale = 0f; // Pause the game
        dieMenu.SetActive(true); // Show the Die screen
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Ensure the game is not paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void Home()
    {
        Time.timeScale = 1f; // Ensure the game is not paused
        SceneManager.LoadScene("MainMenu"); // Load the home screen scene
    }
}
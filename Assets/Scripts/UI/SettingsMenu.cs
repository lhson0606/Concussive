using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject settingsMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputActionReference MoveRef;
    public void Resume()
    {
        settingsMenu.SetActive(false);
        MoveRef.action.Enable();
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        MoveRef.action.Disable();
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Ensure the game is not paused
    }
}

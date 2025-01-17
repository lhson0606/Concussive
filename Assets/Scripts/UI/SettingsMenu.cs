using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    public InputActionReference MoveRef;
    public InputActionAsset inputActions; // Reference to the InputActionAsset

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

    public void ResetKeybindings()
    {
        // Reset all bindings to their default state
        foreach (var map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }
}

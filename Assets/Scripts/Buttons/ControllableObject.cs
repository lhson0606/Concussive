using UnityEngine;

public class ControllableObject : MonoBehaviour, IControllable
{
    // delegate method to be called when the control button state changes
    public event System.Action<bool> OnControlStateChange;

    public void OnControlButtonStateChange(bool isPressed)
    {
        OnControlStateChange?.Invoke(isPressed);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class ButtonsPuzzle : MonoBehaviour
{
    [SerializeField]
    List<ControlButtonComponent> buttons;
    [SerializeField]
    List<ControllableObject> controls;

    private int pressedButtonCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // register the event handler for each button
        foreach (var button in buttons)
        {
            if(button != null)
            {
                button.onButtonStateChange -= OnButtonStateChanged;
                button.onButtonStateChange += OnButtonStateChanged;
            }                
        }

        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonStateChanged(bool isPressed)
    {
        if (isPressed)
        {
            pressedButtonCount++;
        }
        else
        {
            pressedButtonCount--;
        }

        UpdateState();
    }

    private void UpdateState()
    {
        if (pressedButtonCount == buttons.Count)
        {
            foreach (var control in controls)
            {
                if (control != null)
                    control.OnControlButtonStateChange(true);
            }
        }
        else
        {
            foreach (var control in controls)
            {
                if (control != null)
                    control.OnControlButtonStateChange(false);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlButtonComponent : MonoBehaviour
{
    [SerializeField]
    AudioClip buttonTriggerSound;
    [SerializeField]
    List<ControllableObject> controls; // Use System.Collections.Generic.List<IControllable>

    AudioSource audioSource;
    Animator animator;

    private int overlapCount = 0;

    public event Action<bool> onButtonStateChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        foreach (var control in controls)
        {
            control.OnControlButtonStateChange(overlapCount > 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger enter");
        GameObject obj = collision.gameObject;
        IControlButtonInteractable interactable = null;
        if (obj.TryGetComponent<IControlButtonInteractable>(out interactable))
        {
            overlapCount++;
            if (overlapCount == 1)
            {
                OnControlButtonEnter();
            }
        }
    }

    private void OnControlButtonEnter()
    {
        animator.SetBool("isPressed", true);
        if (buttonTriggerSound != null)
        {
            audioSource?.PlayOneShot(buttonTriggerSound);
        }

        // Notify all controls
        foreach (var control in controls)
        {
            control.OnControlButtonStateChange(true);
        }

        // Invoke the event
        onButtonStateChange?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        IControlButtonInteractable interactable = null;
        if (obj.TryGetComponent<IControlButtonInteractable>(out interactable))
        {
            overlapCount--;
            if (overlapCount == 0)
            {
                OnControlButtonExit();
            }
        }
    }

    private void OnControlButtonExit()
    {
        animator.SetBool("isPressed", false);
        if (buttonTriggerSound != null)
        {
            audioSource?.PlayOneShot(buttonTriggerSound);
        }

        // Notify all controls
        foreach (var control in controls)
        {
            control.OnControlButtonStateChange(false);
        }

        // Invoke the event
        onButtonStateChange?.Invoke(false);
    }
}
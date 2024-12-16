using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class BarrierController : MonoBehaviour, IControllable
{
    Animator _animator;
    Collider2D _collider;
    [SerializeField]
    bool isOpen = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _collider.enabled = !isOpen;
        _animator.SetBool("isOpen", isOpen);

        ControllableObject controllableObject = GetComponent<ControllableObject>();
        if(controllableObject != null)
        {
            controllableObject.OnControlStateChange -= OnControlButtonStateChange;
            controllableObject.OnControlStateChange += OnControlButtonStateChange;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        _animator.SetBool("isOpen", true);
    }

    public void Close()
    {
        _animator.SetBool("isOpen", false);
    }

    // call from animation to open barrier
    public void OpenBarrier()
    {
        _collider.enabled = false;
        isOpen = true;
    }

    // call from animation to close barrier
    public void CloseBarrier()
    {
        _collider.enabled = true;
        isOpen = false;
    }

    public void OnControlButtonStateChange(bool isPressed)
    {
        if(isPressed == isOpen)
        {
            return;
        }
        isOpen = isPressed;
        _animator.SetBool("isOpen", isPressed);
    }
}

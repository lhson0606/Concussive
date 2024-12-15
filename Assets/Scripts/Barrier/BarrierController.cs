using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class BarrierController : MonoBehaviour
{
    Animator _animator;
    Collider2D _collider;
    bool _isOpen = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _collider.enabled = !_isOpen;
        _animator.SetBool("isOpen", _isOpen);
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
        _isOpen = true;
    }

    // call from animation to close barrier
    public void CloseBarrier()
    {
        _collider.enabled = true;
        _isOpen = false;
    }
}

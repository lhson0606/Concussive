using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class MapSelectionButton : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseEnter()
    {
        animator.SetBool("Hover", true);
    }

    public void OnMouseExit()
    {
        animator.SetBool("Hover", false);
    }

    public void OnMouseDown()
    {
        animator.SetBool("Pressed", true);
    }
}

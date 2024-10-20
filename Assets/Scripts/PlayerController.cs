using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body = null;
    private Animator animator = null;

    float horizontal;
    float vertical;

    public float runSpeed = 5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        Vector2 moveDir = new Vector2(horizontal, vertical);
        moveDir.Normalize();
        Vector2 moveVector = moveDir * runSpeed * Time.deltaTime;
        body.velocity = moveVector;
        animator.SetFloat("MovingSpeed", moveVector.magnitude);
        animator.SetBool("IsMoving", moveVector.magnitude > 0);
    }
}

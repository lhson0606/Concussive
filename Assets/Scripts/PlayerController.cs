using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body = null;
    private Animator animator = null;
    private SpriteRenderer spriteRenderer = null;
    private bool isEnabled = true;

    float horizontal;
    float vertical;

    public float runSpeed = 5f;

    private void Awake()
    {
        
        
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        body.freezeRotation = true;
    }

    void Update()
    {
        if(!isEnabled)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if(horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    public void ResetMovement()
    {
        body.velocity = Vector2.zero;
        animator.SetFloat("MovingSpeed", 0);
        animator.SetBool("IsMoving", false);
    }

    private void FixedUpdate()
    {
        if(!isEnabled)
        {
            return;
        }

        Vector2 moveDir = new Vector2(horizontal, vertical);
        moveDir.Normalize();
        Vector2 moveVector = moveDir * runSpeed * Time.deltaTime;
        body.velocity = moveVector;
        animator.SetFloat("MovingSpeed", moveVector.magnitude);
        animator.SetBool("IsMoving", moveVector.magnitude > 0);
    }
}

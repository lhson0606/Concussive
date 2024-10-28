using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseCharacter))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body = null;
    private Animator animator = null;
    private SpriteRenderer spriteRenderer = null;
    private bool isEnabled = true;
    private BaseCharacter baseCharacter = null;
    public float pickUpRange = 2.0f; // Define the range within which the player can pick up items
    private PickUpComponent selectedPickUp;
    private AudioSource audioSource;
    private GameObject playerCamera;

    float horizontal;
    float vertical;

    [SerializeField]
    private InputActionReference movement, attack, pointerPosition;

    private void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        animator = baseCharacter.GetComponent<Animator>();
        body = baseCharacter.GetComponent<Rigidbody2D>();
        spriteRenderer = baseCharacter.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerCamera = transform.Find("Main Camera").gameObject;

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

        HandleMouseClick();
        HandlePickUp();
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
        Vector2 moveVector = moveDir * baseCharacter.GetRunSpeed() * Time.deltaTime;
        body.velocity = moveVector;
        animator.SetFloat("MovingSpeed", moveVector.magnitude);
        animator.SetBool("IsMoving", moveVector.magnitude > 0);
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                PickUpComponent pickUpComponent = hit.collider.GetComponent<PickUpComponent>();
                if (pickUpComponent != null)
                {
                    if (selectedPickUp != null)
                    {
                        selectedPickUp.OnDeselect();
                    }
                    selectedPickUp = pickUpComponent;
                    selectedPickUp.OnSelect(GetComponent<BaseCharacter>());
                }
            }
            else
            {
                if (selectedPickUp != null)
                {
                    selectedPickUp.OnDeselect();
                    selectedPickUp = null;
                }
            }
        }
    }

    private void HandlePickUp()
    {
        if (Input.GetKeyDown(KeyCode.E) && selectedPickUp != null)
        {
            float distance = Vector2.Distance(transform.position, selectedPickUp.transform.position);
            if (distance <= pickUpRange)
            {
                selectedPickUp.OnPickUp();
                selectedPickUp = null; // Clear the selection after picking up
            }
        }
    }

    public IPickUpable GetSelectedPickUp()
    {
        return selectedPickUp;
    }

    public bool HasSelectedPickUp()
    {
        return selectedPickUp != null;
    }

    public void SetSelectedPickUp(PickUpComponent pickUpComponent)
    {
        if (selectedPickUp != null)
        {
            selectedPickUp.OnDeselect();
        }        

        selectedPickUp = pickUpComponent;

        if (selectedPickUp != null)
        {
            selectedPickUp.OnSelect(this.GetComponent<BaseCharacter>());
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

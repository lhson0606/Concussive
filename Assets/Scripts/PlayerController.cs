using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseCharacter))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;
    private BaseCharacter baseCharacter = null;
    public float pickUpRange = 2.0f; // Define the range within which the player can pick up items
    private PickUpComponent selectedPickUp;
    private AudioSource audioSource;
    private Camera playerCamera;
    private Vector3 cameraOriginalPosition;

    private Vector2 pointerPosition;

    float horizontal;
    float vertical;
    public delegate void WeaponChangedHandler(BaseWeapon primaryWeapon, BaseWeapon secondaryWeapon);
    public event WeaponChangedHandler OnWeaponChanged;
    private void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
        playerCamera = FindAnyObjectByType<Camera>();
        cameraOriginalPosition = playerCamera.transform.localPosition;
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        spriteRenderer = baseCharacter.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        pointerPosition = GetPointerWorldPosition();
        baseCharacter.LookAtPosition = pointerPosition;
    }

    void Update()
    {
        if(!baseCharacter.CanMove())
        {
            return;
        }

        pointerPosition = GetPointerWorldPosition();
        baseCharacter.LookAtPosition = pointerPosition;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //baseCharacter.SetWeaponPointer(pointerPosition);
        HandleMouseClick();
        HandlePickUp();
        HandleAttack();
        HandleSwitchWeapon();
    }

    private void HandleAttack()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // attack if there is a weapon
            if (baseCharacter.GetPrimaryWeapon() != null)
            {
                baseCharacter.GetPrimaryWeapon().DoAttack();
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(baseCharacter.GetPrimaryWeapon() != null)
            {
                baseCharacter.GetPrimaryWeapon().ReleaseAttack();
            }
        }
    }

    private void HandleSwitchWeapon()
    {
        // Scroll wheel to switch weapons
        if(Input.mouseScrollDelta.y != 0)
        {
            baseCharacter.SwitchToSecondaryWeapon();
            OnWeaponChanged?.Invoke(baseCharacter.GetPrimaryWeapon(), baseCharacter.GetSecondaryWeapon());
        }
    }

    private void FixedUpdate()
    {
        if(!baseCharacter.IsMovementEnabled)
        {
            return;
        }

        if(DialogueManager.GetInstance().dialogueIsPlaying){
            return;
        }

        Vector2 moveDir = new Vector2(horizontal, vertical);
        moveDir.Normalize();
        Vector2 moveVector = moveDir * baseCharacter.GetRunSpeed() * Time.deltaTime;
        baseCharacter.GetRigidbody().linearVelocity = moveVector;
        baseCharacter.GetAnimator().SetFloat("MovingSpeed", moveVector.magnitude);
        baseCharacter.GetAnimator().SetBool("IsMoving", moveVector.magnitude > 0);
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
                BaseWeapon newWeapon = selectedPickUp.GetComponent<BaseWeapon>();
                if (newWeapon != null)
                {
                    OnWeaponChanged?.Invoke(baseCharacter.GetPrimaryWeapon(), baseCharacter.GetSecondaryWeapon());
                }
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

    public Vector2 GetPointerWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    internal void AddCoins(int coinAmount)
    {
        Debug.Log("Player collected " + coinAmount + " coins");
        string text = $"+{coinAmount} coins";
        baseCharacter.SpawnText(text, Color.yellow, 0.5f, Vector2.up);
    }

    public Vector3 GetCameraOriginalPosition()
    {
        return cameraOriginalPosition;
    }
}

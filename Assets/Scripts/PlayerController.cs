using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BaseCharacter))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour, IDataPersistent
{
    private SpriteRenderer spriteRenderer = null;
    private BaseCharacter baseCharacter = null;
    public float pickUpRange = 2.0f; // Define the range within which the player can pick up items
    private PickUpComponent selectedPickUp;
    private AudioSource audioSource;
    private Camera playerCamera;
    private Vector3 cameraOriginalPosition;

    private Vector2 pointerPosition;
    private SkillModule skillModule;
    private Vector2 moveInput;
    float horizontal;
    float vertical;
    public delegate void WeaponChangedHandler(BaseWeapon primaryWeapon, BaseWeapon secondaryWeapon);
    public event WeaponChangedHandler OnWeaponChanged;
    public delegate void CoinsChangedHandler(int amount);
    public event CoinsChangedHandler OnCoinsChanged;

    public int coinsCounter = 10;
    public float switchWeaponCooldown = 0.5f;
    private float switchWeaponCooldownTimer = 0f;

    private void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
        playerCamera = FindAnyObjectByType<Camera>();
        skillModule = GetComponent<SkillModule>();
        cameraOriginalPosition = playerCamera.transform.localPosition;
    }

    private void OnEnable()
    {

    }

    void Start()
    {
        spriteRenderer = baseCharacter.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        //pointerPosition = GetPointerWorldPosition();
        baseCharacter.LookAtPosition = pointerPosition;
        // Initialize coinsCount or other necessary components
    }

    void Update()
    {
        if (!baseCharacter.CanMove())
        {
            return;
        }

        //HandleLook();
        //baseCharacter.SetWeaponPointer(pointerPosition);
        //HandleMouseClick();
        //HandlePickUp();
        //HandleAttack();
        //HandleUseSkill();
        //HandleSwitchWeapon();
        OnWeaponChanged?.Invoke(baseCharacter.GetPrimaryWeapon(), baseCharacter.GetSecondaryWeapon());
        OnCoinsChanged?.Invoke(coinsCounter);

        // Decrement the switch weapon cooldown timer
        if (switchWeaponCooldownTimer > 0)
        {
            switchWeaponCooldownTimer -= Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!baseCharacter.CanMove())
        {
            return;
        }
        Vector2 newmoveInput = context.ReadValue<Vector2>();
        horizontal = newmoveInput.x;
        vertical = newmoveInput.y;
    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        if (!baseCharacter.CanMove())
        {
            return;
        }
        Vector2 mousePos = context.ReadValue<Vector2>();
        pointerPosition = GetPointerWorldPosition(mousePos);
        baseCharacter.LookAtPosition = pointerPosition;
    }

    public void HandleUseSkill(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            skillModule.UseSkill(0);
            float cooldownDuration = 10.0f; // Example cooldown duration
            UIHandler.instance.OnSpecialAbilityUsed(cooldownDuration);
        }
    }

    public void HandleAttack(InputAction.CallbackContext context)
    {
        if(!baseCharacter.CanMove())
        {
            return;
        }

        if (context.phase == InputActionPhase.Started)
        {
            // attack if there is a weapon
            if (baseCharacter.GetPrimaryWeapon() != null)
            {
                baseCharacter.GetPrimaryWeapon().DoAttack();
            }
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            if (baseCharacter.GetPrimaryWeapon() != null)
            {
                baseCharacter.GetPrimaryWeapon().ReleaseAttack();
            }
        }
    }

    public void HandleSwitchWeapon(InputAction.CallbackContext context)
    {
        if(!baseCharacter.CanMove())
        {
            return;
        }

        if (switchWeaponCooldownTimer <= 0)
        {
            float scrollValue = context.ReadValue<Vector2>().y;

            // Scroll wheel to switch weapons
            if (scrollValue != 0)
            {
                baseCharacter.SwitchToSecondaryWeapon();
                OnWeaponChanged?.Invoke(baseCharacter.GetPrimaryWeapon(), baseCharacter.GetSecondaryWeapon());
                switchWeaponCooldownTimer = switchWeaponCooldown; // Reset the cooldown timer
            }
        }
    }

    public void HandleChangeWeaponMode(InputAction.CallbackContext context)
    {
        if(!baseCharacter.CanMove())
        {
            return;
        }

        HybridWeapon hybridWeapon = baseCharacter.GetPrimaryWeapon() as HybridWeapon;

        if(hybridWeapon != null)
        {
            hybridWeapon.OnSpecialModeTriggered();
        }
    }

    private void FixedUpdate()
    {
        if (!baseCharacter.IsMovementEnabled)
        {
            return;
        }

        if (DialogueManager.GetInstance() && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        Vector2 moveDir = new Vector2(horizontal, vertical);
        moveDir.Normalize();
        Vector2 moveVector = moveDir * baseCharacter.GetRunSpeed();
        baseCharacter.GetRigidbody().linearVelocity = moveVector;
    }

    public void HandleMouseClick(InputAction.CallbackContext context)
    {
        if(!baseCharacter.CanMove())
        {
            return;
        }

        if (context.phase == InputActionPhase.Started) // Left mouse button
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

    public void HandlePickUp(InputAction.CallbackContext context)
    {
        if(!baseCharacter.CanMove() || !selectedPickUp)
        {
            return;
        }

        if (context.phase == InputActionPhase.Started)
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

    public Vector2 GetPointerWorldPosition(Vector3 mousePos)
    {
        if(Camera.main == null)
        {
            return Vector2.zero;
        }
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    internal void AddCoins(int coinAmount)
    {
        Debug.Log("Player collected " + coinAmount + " coins");
        string text = $"+{coinAmount} coins";
        baseCharacter.SpawnText(text, Color.yellow, 0.5f, Vector2.up);
        coinsCounter += coinAmount;

    }

    public void RemoveCoins(int amount)
    {

    }

    public Vector3 GetCameraOriginalPosition()
    {
        return cameraOriginalPosition;
    }

    public void ShakePlayerCamera(Vector3 pos, float shakeDuration = 0.5f, float maxShakeMagnitude = 0.4f, float shakeDistanceThreshold = 10.0f)
    {
        float distance = Vector3.Distance(transform.position, pos);

        if (distance <= shakeDistanceThreshold)
        {
            StartCoroutine(CameraShake(distance, shakeDuration, maxShakeMagnitude, shakeDistanceThreshold));
        }
    }

    public void ShakePlayerCamera(float distance, float shakeDuration = 0.5f, float maxShakeMagnitude = 0.4f, float shakeDistanceThreshold = 10.0f)
    {
        if (distance <= shakeDistanceThreshold)
        {
            StartCoroutine(CameraShake(distance, shakeDuration, maxShakeMagnitude, shakeDistanceThreshold));
        }
    }

    private IEnumerator CameraShake(float distance, float shakeDuration = 0.5f, float maxShakeMagnitude = 0.4f, float shakeDistanceThreshold = 10.0f)
    {
        float elapsed = 0.0f;

        // Calculate shake magnitude based on distance
        float shakeMagnitude = maxShakeMagnitude * (1 - (distance / shakeDistanceThreshold));

        while (elapsed < shakeDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.transform.localPosition = new Vector3(cameraOriginalPosition.x + x, cameraOriginalPosition.y + y, cameraOriginalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        playerCamera.transform.localPosition = cameraOriginalPosition;
    }

    // return vector zero if no input
    // else return the major input axis direction Up/Down/Left/Right
    public Vector2 GetMajorInputAxisDirection()
    {
        if (horizontal == 0 && vertical == 0)
        {
            return Vector2.zero;
        }
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            return new Vector2(Mathf.Sign(horizontal), 0).normalized;
        }
        else
        {
            return new Vector2(0, Mathf.Sign(vertical)).normalized;
        }
    }

    public void LoadData(GameData gameData)
    {
        Debug.Log("Loading player data.");
        if (gameData == null)
        {
            return;
        }

        GameObject savedPrimaryWeaponPrefab = null;
        GameObject savedSecondaryWeaponPrefab = null;

        if (gameData.primaryWeapon != "")
        {
            savedPrimaryWeaponPrefab = Resources.Load<GameObject>("Weapons/" + gameData.primaryWeapon);
        }

        if (gameData.secondaryWeapon != "")
        {
            savedSecondaryWeaponPrefab = Resources.Load<GameObject>("Weapons/" + gameData.secondaryWeapon);
        }

        baseCharacter.SetInitialWeaponPrefabs(savedPrimaryWeaponPrefab, savedSecondaryWeaponPrefab);

        baseCharacter.MaxHealth = gameData.maxHealth;
        baseCharacter.CurrentHealth = gameData.currentHealth;
        baseCharacter.SetMaxMana(gameData.maxMana);
        baseCharacter.MaxArmor = gameData.maxArmor;
        coinsCounter = gameData.coinAmount;
    }

    public void SaveData(GameData gameData)
    {
        if (gameData == null)
        {
            return;
        }

        if (baseCharacter.GetPrimaryWeapon() != null)
        {
            string primaryWeaponName = baseCharacter.GetPrimaryWeapon().name;
            gameData.primaryWeapon = primaryWeaponName.Replace("(Clone)", "").Trim();
        }

        if (baseCharacter.GetSecondaryWeapon() != null)
        {
            string secondaryWeaponName = baseCharacter.GetSecondaryWeapon().name;
            gameData.secondaryWeapon = secondaryWeaponName.Replace("(Clone)", "").Trim();
        }

        gameData.currentScene = SceneManager.GetActiveScene().name;

        gameData.maxHealth = baseCharacter.MaxHealth;
        gameData.currentHealth = baseCharacter.CurrentHealth;
        gameData.maxMana = baseCharacter.GetMaxMana();
        gameData.maxArmor = baseCharacter.MaxArmor;
        gameData.coinAmount = coinsCounter;
    }
}

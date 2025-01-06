using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BaseCharacter))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour,IDataPersistent
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

    float horizontal;
    float vertical;
    public delegate void WeaponChangedHandler(BaseWeapon primaryWeapon, BaseWeapon secondaryWeapon);
    public event WeaponChangedHandler OnWeaponChanged;
    public delegate void CoinsChangedHandler(int amount);
    public event CoinsChangedHandler OnCoinsChanged;

    public int coinsCounter = 10;

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
        pointerPosition = GetPointerWorldPosition();
        baseCharacter.LookAtPosition = pointerPosition;
        // Initialize coinsCount or other necessary components
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
        HandleUseSkill();
        HandleSwitchWeapon();
        OnWeaponChanged?.Invoke(baseCharacter.GetPrimaryWeapon(), baseCharacter.GetSecondaryWeapon());
        OnCoinsChanged?.Invoke(coinsCounter);
    }

    private void HandleUseSkill()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            skillModule.UseSkill(0);
            float cooldownDuration = 10.0f; // Example cooldown duration
            UIHandler.instance.OnSpecialAbilityUsed(cooldownDuration);
        }
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

        if(DialogueManager.GetInstance() && DialogueManager.GetInstance().dialogueIsPlaying){
            return;
        }

        Vector2 moveDir = new Vector2(horizontal, vertical);
        moveDir.Normalize();
        Vector2 moveVector = moveDir * baseCharacter.GetRunSpeed();
        baseCharacter.GetRigidbody().linearVelocity = moveVector;
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

        if(Input.GetMouseButton(1)) // Right mouse button
        {
            if(baseCharacter.GetPrimaryWeapon()!=null)
            {
                baseCharacter.GetPrimaryWeapon().OnSpecialModeTriggered();
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private VisualElement UI_Healthbar;
    private VisualElement UI_PrimaryWeapon;
    private VisualElement UI_SecondadryWeapon;
    public static UIHandler instance { get; private set; }

    private BaseCharacter baseCharacter; // Class-level variable

    // Awake is called when the script instance is being loaded (in this situation, when the game scene loads)
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        UI_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("Health");
        UI_PrimaryWeapon = uiDocument.rootVisualElement.Q<VisualElement>("PrimaryWeapon");
        UI_SecondadryWeapon = uiDocument.rootVisualElement.Q<VisualElement>("SecondaryWeapon");

        // Find the player GameObject with the tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Get the PlayerController component from the player GameObject
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                baseCharacter = playerController.GetComponent<BaseCharacter>();
                // Set the health value using the player's currentHealth and maxHealth
                SetHealthValue(baseCharacter.CurrentHealth / (float)baseCharacter.MaxHealth);

                // Subscribe to the OnWeaponChanged event
                playerController.OnWeaponChanged += OnWeaponChanged;

                // Set the initial weapon sprites if the character has weapons
                if (baseCharacter.PrimaryWeapon != null)
                {
                    SetWeaponSprite(UI_PrimaryWeapon, baseCharacter.PrimaryWeapon.GetWeaponSpriteRenderer().sprite);
                }
                if (baseCharacter.GetSecondaryWeapon() != null)
                {
                    SetWeaponSprite(UI_SecondadryWeapon, baseCharacter.GetSecondaryWeapon().GetWeaponSpriteRenderer().sprite);
                }
            }
            else
            {
                Debug.LogError("PlayerController component not found on the player GameObject.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }

        Debug.Log("Healthbar: ");
    }

    // Update is called once per frame
    void Update()
    {
        if (baseCharacter != null)
        {
            // Set the health value using the player's currentHealth and maxHealth
            SetHealthValue(baseCharacter.CurrentHealth / (float)baseCharacter.MaxHealth);
        }
    }

    public void SetHealthValue(float percentage)
    {
        if (UI_Healthbar != null)
        {
            UI_Healthbar.style.width = Length.Percent(100 * percentage);
        }
        else
        {
            Debug.LogError("UI_Healthbar is null. Cannot set health value.");
        }
    }

    public void SetWeaponSprite(VisualElement weaponElement, Sprite weaponSprite)
    {
        if (weaponElement != null)
        {
            // Use the sprite directly as the background image
            weaponElement.style.backgroundImage = new StyleBackground(weaponSprite);
        }
        else
        {
            Debug.LogError("Weapon element is null. Cannot set weapon sprite.");
        }
    }

    // Event handler for weapon change event
    private void OnWeaponChanged(BaseWeapon primaryWeapon, BaseWeapon secondaryWeapon)
    {
        if (primaryWeapon != null)
        {
            SetWeaponSprite(UI_PrimaryWeapon, primaryWeapon.GetWeaponSpriteRenderer().sprite);
        }
        if (secondaryWeapon != null)
        {
            SetWeaponSprite(UI_SecondadryWeapon, secondaryWeapon.GetWeaponSpriteRenderer().sprite);
        }
    }
}

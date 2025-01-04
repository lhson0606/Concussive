using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement UI_Healthbar;
    private VisualElement UI_PrimaryWeapon;
    private VisualElement UI_SecondaryWeapon;
    private VisualElement SkillIcon;
    private VisualElement SkillCooldownOverlay;
    public static UIHandler instance { get; private set; }

    private BaseCharacter baseCharacter; // Class-level variable
    private Label UI_CoinsCounter;
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
        UI_SecondaryWeapon = uiDocument.rootVisualElement.Q<VisualElement>("SecondaryWeapon");
        SkillIcon = uiDocument.rootVisualElement.Q<VisualElement>("Skill");
        UI_CoinsCounter = uiDocument.rootVisualElement.Q<Label>("CoinsCounter");
        SkillCooldownOverlay = new VisualElement();
        SkillCooldownOverlay.style.backgroundColor = new StyleColor(Color.gray);
        SkillCooldownOverlay.style.position = Position.Absolute;
        SkillCooldownOverlay.style.top = 0;
        SkillCooldownOverlay.style.left = 0;
        SkillCooldownOverlay.style.right = 0;
        SkillCooldownOverlay.style.bottom = 0;
        SkillCooldownOverlay.style.height = Length.Percent(0);
        SkillIcon.Add(SkillCooldownOverlay);
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
                    SetWeaponSprite(UI_SecondaryWeapon, baseCharacter.GetSecondaryWeapon().GetWeaponSpriteRenderer().sprite);
                }
                playerController.OnCoinsChanged += OnCoinsChanged;

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

        // Initialize the coins count display


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
            SetWeaponSprite(UI_SecondaryWeapon, secondaryWeapon.GetWeaponSpriteRenderer().sprite);
        }
    }
    private void OnCoinsChanged(int amount)
    {
        UI_CoinsCounter.text = amount.ToString();
    }

    // Method to update the skill cooldown visual element
    public void OnSpecialAbilityUsed(float cooldownDuration)
    {
        StartCoroutine(HandleSkillCooldown(cooldownDuration));
    }

    private IEnumerator HandleSkillCooldown(float cooldownDuration)
    {
        float elapsedTime = 0f;

        // Set the overlay height to full at the start
        SkillCooldownOverlay.style.top = Length.Percent(0);
        SkillCooldownOverlay.style.height = Length.Percent(100);

        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / cooldownDuration;
            SkillCooldownOverlay.style.top = Length.Percent(percentage * 100);
            SkillCooldownOverlay.style.height = Length.Percent(100 - (percentage * 100));
            yield return null;
        }

        // Reset the overlay height
        SkillCooldownOverlay.style.top = Length.Percent(0);
        SkillCooldownOverlay.style.height = Length.Percent(0);
    }
}

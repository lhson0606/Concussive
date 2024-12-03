using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private VisualElement Weapon;
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
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("Health");
        Weapon = uiDocument.rootVisualElement.Q<VisualElement>("Weapon");

        // Find the player GameObject with the tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Get the BaseCharacter component from the player GameObject
            baseCharacter = player.GetComponent<BaseCharacter>();
            if (baseCharacter != null)
            {
                // Set the health value using the player's currentHealth and maxHealth
                SetHealthValue(baseCharacter.CurrentHealth / (float)baseCharacter.MaxHealth);

                // Set the initial weapon sprite if the character has a primary weapon
            }
            else
            {
                Debug.LogError("BaseCharacter component not found on the player GameObject.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }

        Debug.Log("Healthbar: ");
    }

    // Update is called once per frame

    public void SetHealthValue(float percentage)
    {
        if (m_Healthbar != null)
        {
            m_Healthbar.style.width = Length.Percent(100 * percentage);
        }
        else
        {
            Debug.LogError("m_Healthbar is null. Cannot set health value.");
        }
    }

    public void SetWeaponSprite(Sprite weaponSprite)
    {
        if (Weapon != null)
        {
            // Convert the sprite to a texture
            Texture2D texture = SpriteToTexture2D(weaponSprite);

            if (texture != null)
            {
                // Set the texture to the UI element
                Weapon.style.backgroundImage = new StyleBackground(texture);
            }
            else
            {
                Debug.LogError("Failed to convert sprite to texture.");
            }
        }
        else
        {
            Debug.LogError("Weapon is null. Cannot set weapon sprite.");
        }
    }

    private Texture2D SpriteToTexture2D(Sprite sprite)
    {
        try
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
            {
                return sprite.texture;
            }
        }
        catch (UnityException e)
        {
            Debug.LogError("Texture is not readable: " + e.Message);
            return null;
        }
    }
}

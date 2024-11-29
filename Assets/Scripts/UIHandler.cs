using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIHandler : MonoBehaviour
{
   private VisualElement m_Healthbar;
   private VisualElement Weapon;
   public static UIHandler instance { get; private set; }


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
       Debug.Log("Healthbar: " );
   }




   public void SetHealthValue(float percentage)
   {
       m_Healthbar.style.width = Length.Percent(100 * percentage);


   }
    public void SetWeaponSprite(Sprite weaponSprite)
    {
        // Convert the sprite to a texture
        Texture2D texture = SpriteToTexture2D(weaponSprite);

        // Set the texture to the UI element
        Weapon.style.backgroundImage = new StyleBackground(texture);
    }

    private Texture2D SpriteToTexture2D(Sprite sprite)
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
            return sprite.texture;
    }

}

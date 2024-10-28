using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameItem : MonoBehaviour
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private ItemRarity rarity;
    [SerializeField]
    private string description;
    [SerializeField]
    private GameObject uiTextPrefab;
    [SerializeField]
    private Sprite icon = null;
    [SerializeField]
    private Transform onGroundTransform = null;

    public string ItemName => itemName;
    public ItemRarity Rarity => rarity;
    public string Description => description;
    protected PlayerController playerController = null;
    private GameObject uiTextInstance = null;

    protected void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Method to get the color based on rarity
    public Color GetRarityColor()
    {
        RarityConfig.LoadInstance();
        return RarityConfig.Instance.GetColorForRarity(rarity);
    }

    public Color GetRaritySecondaryColor()
    {
        RarityConfig.LoadInstance();
        return RarityConfig.Instance.GetSecondaryColorForRarity(rarity);
    }

    // Method to display item information
    public virtual void DisplayInfo()
    {
        Debug.Log($"Item: {itemName}\nRarity: {rarity}\nDescription: {description}");
    }

    // Method to display the item name on a UI Text component at the GameItem's position
    public void DisplayItemName()
    {
        if(uiTextInstance != null)
        {
            return;
        }

        if (uiTextPrefab != null)
        {
            // Instantiate the UI Text prefab
            uiTextInstance = Instantiate(uiTextPrefab, gameObject.transform.position, Quaternion.identity);
            try
            {
                GameObject text = uiTextInstance.transform.Find("Canvas").transform.Find("Text").gameObject;
                TextControl textControl = text.GetComponent<TextControl>();
                textControl.SetText(itemName);
                textControl.SetColor(GetRarityColor());
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("UI Text prefab does not have a child named 'Canvas' with a child named 'Text'.");
                Debug.LogWarning(e.Message);
            }

            // Convert the GameItem's world position to screen position
            //Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            //uiTextInstance.transform.position = screenPosition;
        }
        else
        {
            Debug.LogWarning("UI Text prefab is not assigned.");
        }
    }

    // Method to stop displaying the item name
    public void StopDisplayingItemName()
    {
        if (uiTextInstance != null)
        {
            Destroy(uiTextInstance.gameObject);
            uiTextInstance = null;
        }
    }

    public Transform GetOnGroundTransform()
    {
        return onGroundTransform;
    }
}

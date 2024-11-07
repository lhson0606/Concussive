using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameItem : SlowMotionObject
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private ItemRarity rarity;
    [SerializeField]
    private string description;
    [SerializeField]
    private GameObject uiTextPrefab;

    public string ItemName => itemName;
    public ItemRarity Rarity => rarity;
    public string Description => description;
    protected PlayerController playerController = null;
    protected GameObject uiTextInstance = null;
    protected PickUpComponent pickUpComponent = null;

    protected virtual void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    protected override void Awake()
    {
        base.Awake();
        SetUpPickUp();
    }

    protected virtual void OnValidate()
    {
    }

    protected virtual void SetUpPickUp()
    {
        pickUpComponent = GetComponent<PickUpComponent>();
        if (pickUpComponent != null)
        {
            // Unsubscribe first to prevent multiple subscriptions
            pickUpComponent.OnPickedUp -= OnPickUp;
            pickUpComponent.OnPickedUp += OnPickUp;
        }
    }

    public virtual void DropItem(Vector3 position)
    {
        //check if it contains PickUpComponent
        if(pickUpComponent == null)
        {
            pickUpComponent = gameObject.AddComponent<PickUpComponent>();
        }

        //set the item to be on the ground
        SetOnGround(position);
        SetUpPickUp();
    }

    protected virtual void SetOnGround(Vector3 position)
    {
        transform.position = position;
        transform.SetParent(null);
        SetUpPickUp();
    }

    protected virtual void OnPickUp(BaseCharacter baseCharacter)
    {
        Debug.Log("GameItem picked up");
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (pickUpComponent != null)
        {
            pickUpComponent.OnPickedUp -= OnPickUp;
        }
    }
}

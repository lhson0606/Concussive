using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private GameObject ItemImage;
    [SerializeField] private GameObject ItemName;
    [SerializeField] private GameObject ItemPrice;

    public string itemName;
    public int price;
    public GameObject pickUpPrefab; // Prefab for the PickUpComponent

    private PickUpComponent selectedPickUp;

    void Start()
    {
        // Set the item name
        TextMeshProUGUI nameTextComponent = ItemName.GetComponent<TextMeshProUGUI>();
        if (nameTextComponent != null)
        {
            nameTextComponent.text = itemName;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found");
        }

        // Set the item price
        TextMeshProUGUI priceTextComponent = ItemPrice.GetComponent<TextMeshProUGUI>();
        if (priceTextComponent != null)
        {
            priceTextComponent.text = price.ToString();
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found");
        }

        // Set the item image from the pickUpPrefab
        PickUpComponent pickUpComponent = pickUpPrefab.GetComponent<PickUpComponent>();
        if (pickUpComponent != null)
        {
            SpriteRenderer spriteRenderer = pickUpComponent.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                ItemImage.GetComponent<Image>().sprite = spriteRenderer.sprite;
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found in pickUpPrefab");
            }
        }
        else
        {
            Debug.LogError("PickUpComponent not found on pickUpPrefab");
        }
    }

    public void OnItemClick()
    {
        // Find the player and get their position
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Vector3 playerPosition = playerController.transform.position;

            // Instantiate the pickUpPrefab at the player's position
            GameObject pickUpObject = Instantiate(pickUpPrefab, playerPosition, Quaternion.identity);
            selectedPickUp = pickUpObject.GetComponent<PickUpComponent>();

            if (selectedPickUp == null)
            {
                Debug.LogError("PickUpComponent not found on the instantiated prefab");
                return;
            }

            // Automatically pick up the item
            selectedPickUp.OnPickUp();
        }
        else
        {
            Debug.LogError("PlayerController not found in the scene");
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public GameItem pickUpPrefab;
    public string ItemName;
    public string ItemPrice;

    private Image itemImage;
    private TextMeshProUGUI itemNameText;
    private TextMeshProUGUI itemPriceText;
    public GameObject overlay; // Reference to the overlay UI element
    private bool isBought = false;
    private GameObject panel;
    private void Start()
    {
        // Find the Image component within the nested hierarchy
        Transform imageTransform = transform.Find("Image/Item");
        if (imageTransform == null)
        {
            Debug.LogError("GameObject 'Image/Item' not found in the hierarchy");
        }
        else
        {
            itemImage = imageTransform.GetComponent<Image>();
            if (itemImage == null)
            {
                Debug.LogError("Image component not found on GameObject 'Image/Item'");
            }
            else
            {
                // Get the SpriteRenderer component from the pickUpPrefab
                SpriteRenderer spriteRenderer = pickUpPrefab.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // Update the itemImage with the sprite from the SpriteRenderer
                    itemImage.sprite = spriteRenderer.sprite;
                }
                else
                {
                    Debug.LogError("SpriteRenderer component not found on pickUpPrefab");
                }
            }
        }

        // Find and set the text components
        itemNameText = transform.Find("ShopInfo/Name").GetComponent<TextMeshProUGUI>();
        itemPriceText = transform.Find("ShopInfo/Price").GetComponent<TextMeshProUGUI>();
        panel = transform.Find("Panel").gameObject;

        if (itemNameText != null)
        {
            itemNameText.text = ItemName;
        }
        if (itemPriceText != null)
        {
            itemPriceText.text = ItemPrice;
        }
    }

    public void OnItemClick()
    {
        if (isBought)
        {
            Debug.LogError("Item has already been bought");
            return;
        }
        // Find the player and get their position
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController.coinsCounter >= int.Parse(ItemPrice))
        {
            Vector3 playerPosition = playerController.transform.position;

            GameItem newItem = Instantiate(pickUpPrefab, playerPosition, Quaternion.identity);

            newItem.DropItem(playerPosition);
            playerController.coinsCounter -= int.Parse(ItemPrice);
            isBought = true;
            panel.SetActive(true); // Assuming the panel is initially inactive
        }
        else
        {
            Debug.LogError("Not enough coins to purchase item");
        }
    }
}

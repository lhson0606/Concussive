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
        itemNameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemPriceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();

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
        // Find the player and get their position
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController.coinsCounter >= int.Parse(ItemPrice))
        {
            Vector3 playerPosition = playerController.transform.position;

            GameItem newItem = Instantiate(pickUpPrefab, playerPosition, Quaternion.identity);

            newItem.DropItem(playerPosition);
            playerController.coinsCounter -= int.Parse(ItemPrice);
        }
        else
        {
            Debug.LogError("Not enough coins to purchase item");
        }
    }
}

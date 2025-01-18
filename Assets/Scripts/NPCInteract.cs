using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] private GameObject shopUIPanel;
    private bool isPlayerInRange = false;
    private bool isShopOpen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseShop();
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isShopOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }
    }

    private void OpenShop()
    {
        ShopMenu shopMenu = shopUIPanel.GetComponent<ShopMenu>();
        if (shopMenu != null)
        {
            shopMenu.Shopping();
            isShopOpen = true;
        }
        else
        {
            //Debug.LogError("ShopMenu component not found on shopUIPanel");
        }
    }

    private void CloseShop()
    {
        ShopMenu shopMenu = shopUIPanel.GetComponent<ShopMenu>();
        if (shopMenu != null)
        {
            shopMenu.Exit();
            isShopOpen = false;
        }
        else
        {
            //Debug.LogError("ShopMenu component not found on shopUIPanel");
        }
    }
}
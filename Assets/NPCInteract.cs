using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] private GameObject shopUIPanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            OpenShop();
    }

    private void OpenShop()
    {
        ShopMenu shopMenu = shopUIPanel.GetComponent<ShopMenu>();
        if (shopMenu != null)
        {
            shopMenu.Shopping();
        }
        else
        {
            Debug.LogError("ShopMenu component not found on shopUIPanel");
        }
    }

    private void CloseShop()
    {
        ShopMenu shopMenu = shopUIPanel.GetComponent<ShopMenu>();
        if (shopMenu != null)
        {
            shopMenu.Exit();
        }
        else
        {
            Debug.LogError("ShopMenu component not found on shopUIPanel");
        }
    }
}

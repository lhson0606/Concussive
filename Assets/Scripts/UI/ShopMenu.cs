using UnityEngine;
using UnityEngine.SceneManagement;
public class ShopMenu : MonoBehaviour
{
    [SerializeField] GameObject shopMenu;
    public void Shopping()
    {
         // Pause the game
        shopMenu.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f; // Resume the game
        shopMenu.SetActive(false);
    }
}

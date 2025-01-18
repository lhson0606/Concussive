using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ContinueMenu : MonoBehaviour
{
    public TMP_Text levelNameText;
    public TMP_Text coinText;
    public Image primaryWeaponImage;
    public Image secondaryWeaponImage;
    private GameData gameData;
    private SpriteRenderer primaryWeaponSpriteRenderer;
    private SpriteRenderer secondaryWeaponSpriteRenderer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameData = DataPersistentManager.instance.getGameData();
        levelNameText.text = gameData.currentScene;

        LoadWeaponSprites();
        coinText.text = gameData.coinAmount.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameData.currentScene);
    }

    private void LoadWeaponSprites()
    {

        if(gameData.primaryWeapon != "")
        {
            GameObject primaryWeapon = Resources.Load<GameObject>("Weapons/" + gameData.primaryWeapon);
            primaryWeaponSpriteRenderer = primaryWeapon.GetComponent<SpriteRenderer>();
            primaryWeaponImage.sprite = primaryWeaponSpriteRenderer.sprite;
        }

        if(gameData.secondaryWeapon != "")
        {
            GameObject secondaryWeapon = Resources.Load<GameObject>("Weapons/" + gameData.secondaryWeapon);
            secondaryWeaponSpriteRenderer = secondaryWeapon.GetComponent<SpriteRenderer>();
            secondaryWeaponImage.sprite = secondaryWeaponSpriteRenderer.sprite;
        }
    }
    


}

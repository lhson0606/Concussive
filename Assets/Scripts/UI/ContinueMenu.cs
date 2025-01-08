using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ContinueMenu : MonoBehaviour
{
    public TMP_Text levelNameText;
    private GameData gameData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameData = DataPersistentManager.instance.getGameData();
        levelNameText.text = gameData.currentScene;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameData.currentScene);
    }

    


}

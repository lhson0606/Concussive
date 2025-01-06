using UnityEngine;

[System.Serializable]
public class GameData
{
    public string primaryWeapon;
    public string secondaryWeapon;
    public string currentScene;

    public GameData()
    {
        primaryWeapon = "";
        secondaryWeapon = "";
        currentScene = "";
    }
}

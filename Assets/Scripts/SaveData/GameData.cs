using UnityEngine;

[System.Serializable]
public class GameData
{
    public string primaryWeapon;
    public string secondaryWeapon;
    public string currentScene;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int maxArmor;

    public GameData()
    {
        primaryWeapon = "";
        secondaryWeapon = "";
        currentScene = "";
        maxHealth = 50;
        currentHealth = 50;
        maxMana = 200;
        maxArmor = 32;
    }
}

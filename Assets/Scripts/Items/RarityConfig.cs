using UnityEngine;

[CreateAssetMenu(fileName = "RarityConfig", menuName = "Scriptable Objects/RarityConfig")]
public class RarityConfig : ScriptableObject
{
    public static RarityConfig Instance { get; private set; }

    public RarityColor[] rarityColors;

    [System.Serializable]
    public struct RarityColor
    {
        public ItemRarity rarity;
        public Color color;
        public Color secondaryColor;
    }

    public static void LoadInstance()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<RarityConfig>("RarityConfig");
            if (Instance == null)
            {
                Debug.LogError("RarityConfig: No RarityConfig asset found in Resources folder.");
            }
        }
    }

    public Color GetColorForRarity(ItemRarity rarity)
    {
        foreach (var rarityColor in rarityColors)
        {
            if (rarityColor.rarity == rarity)
            {
                return rarityColor.color;
            }
        }
        return Color.white; // Default color if rarity not found
    }

    public Color GetSecondaryColorForRarity(ItemRarity rarity)
    {
        foreach (var rarityColor in rarityColors)
        {
            if (rarityColor.rarity == rarity)
            {
                return rarityColor.secondaryColor;
            }
        }
        return Color.white; // Default color if rarity not found
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "EffectConfig", menuName = "Scriptable Objects/EffectConfig")]
public class EffectConfig : ScriptableObject
{
    private static EffectConfig instance;

    public static EffectConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EffectConfig>("EffectConfig");
                if (instance == null)
                {
                    Debug.LogError("EffectConfig: No EffectConfig asset found in Resources folder.");
                }
            }
            return instance;
        }
    }

    public EffectData[] effects;

    [System.Serializable]
    public struct EffectData
    {
        public EffectType effectType;
        public Color textColor;
    }

    public Color GetEffectTextColor(EffectType effectType)
    {
        foreach (var effectData in effects)
        {
            if (effectData.effectType == effectType)
            {
                return effectData.textColor;
            }
        }
        return Color.white; // Default color if effect type not found
    }
}
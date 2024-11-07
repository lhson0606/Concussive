using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Scriptable Objects/Buff")]
public class Buff : ScriptableObject
{
    public BuffType type;
    public float value;
    public string buffName;
    public Sprite icon;
    public string description;
    public bool canStack;
}

using UnityEngine;

public class DamageData
{
    public Element SourceElement { get; set; } = null;
    public DamageType SourceDamageType { get; set; } = DamageType.PHYSIC;
    public bool IsCritical { get; set; } = false;
    public float Damage { get; set; } = 0f;
    public Vector2 SourcePosition { get; set; } = Vector2.zero;
    public Vector2 TargetPosition { get; set; } = Vector2.zero;
}

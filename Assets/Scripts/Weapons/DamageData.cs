using UnityEngine;

public class DamageData
{
    public Element SourceElement { get; set; } = null;
    public DamageType SourceDamageType { get; set; } = DamageType.PHYSIC;
    public bool IsCritical { get; set; } = false;
    public float Damage { get; set; } = 0f;
}

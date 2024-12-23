using UnityEngine;

public class HybridWeapon : BaseWeapon
{
    public enum HybridMode
    {
        Melee,
        Ranged,
    }

    [SerializeField]
    protected HybridMode mode = HybridMode.Melee;
}

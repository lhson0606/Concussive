using UnityEngine;

public class PoisonI : Effect
{
    [SerializeField]
    float damagePerTick = 3f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        target?.TakeDirectEffectDamage((int)damagePerTick, this);
    }
}

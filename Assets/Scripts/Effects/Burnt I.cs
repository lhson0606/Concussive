using UnityEngine;

public class BurntI : Effect
{
    [SerializeField]
    float damagePercentPerTick = 3f;
    [SerializeField]
    int maxDamagePerTick = 99;
    [SerializeField]
    int minDamagePerTick = 1;

    private int damagePerTick;

    public override void StartEffect(BaseCharacter target)
    {
        base.StartEffect(target);
        damagePerTick = CalculateDamage();
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        target.TakeDirectEffectDamage(damagePerTick, this);
    }

    private int CalculateDamage()
    {
        int targetMaxHealth = target.MaxHealth;

        return Mathf.Clamp(Mathf.RoundToInt(targetMaxHealth * damagePercentPerTick / 100), minDamagePerTick, maxDamagePerTick);
    }
}

using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(DamageData damageData, bool isInvisible = false);

    public void TakeDirectEffectDamage(int amount, Effect effect, bool isInvisible = false);
}

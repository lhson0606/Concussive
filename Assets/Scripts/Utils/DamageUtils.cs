using UnityEngine;

public class DamageUtils
{
    public static bool TryToApplyDamageTo(GameObject damageDealer, Collider2D collider, DamageSource damageSource, bool shouldApplyCoolDown = true)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return false;
        }
        BaseCharacter target = collider.GetComponent<BaseCharacter>();
        if (target != null)
        {
            damageSource.ApplyDamageTo(target, damageDealer.transform.position, shouldApplyCoolDown);
            return true;
        }
        else
        {
            damageable.TakeDamage(DamageData.empty);
            return true;
        }
    }

    public static bool TryToApplyDamageDataTo(GameObject damageDealer, Collider2D collider, DamageData damageData, DamageSource damageSource, bool shouldApplyCoolDown = true)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return false;
        }
        BaseCharacter target = collider.GetComponent<BaseCharacter>();
        if (target != null)
        {
            damageSource.ApplyDamageTo(target, damageDealer.transform.position, shouldApplyCoolDown);
            return true;
        }
        else
        {
            damageable.TakeDamage(DamageData.empty);
            return true;
        }
    }
}

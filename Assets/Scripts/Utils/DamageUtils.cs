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

    /*
     * Like TryToApplyDamageTo but returns the amount of damage applied.
     * returns -1 if the target is not damageable
     * returns 0 if the target is damageable but not a BaseCharacter
     * return the amount of damage applied if the target is a BaseCharacter
     */
    public static int TryToApplyDamageToWithResult(GameObject damageDealer, Collider2D collider, DamageSource damageSource, bool shouldApplyCoolDown = true)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return -1;
        }
        BaseCharacter target = collider.GetComponent<BaseCharacter>();
        if (target != null)
        {
            var damageData = damageSource.ApplyDamageTo(target, damageDealer.transform.position, shouldApplyCoolDown);
            return (int)damageData.Damage;
        }
        else
        {
             damageable.TakeDamage(DamageData.empty);
            return 0;
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
            damageable.TakeDamage(damageData);
            if(damageData.IsCritical && damageData.SourceElement.IsElemental)
            {
                damageSource.ApplyElementalEffectToTarget(target);
            }
            return true;
        }
        else
        {
            damageable.TakeDamage(DamageData.empty);
            return true;
        }
    }
}

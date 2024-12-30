using UnityEngine;

public class Sandbag : BaseCharacter
{
    public override void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        base.TakeDamage(damageData, true);
        Vector2 direction = (damageData.SourcePosition - damageData.TargetPosition).normalized;

        if(direction.x >0 )
        {
            GetComponent<Animator>()?.SetBool("IsHurtingRight", true);
        }
        else
        {
            GetComponent<Animator>()?.SetBool("IsHurtingLeft", true);
        }
    }

    public override void TakeDirectEffectDamage(int amount, Effect effect, bool ignoreArmor = false, bool isInvisible = false)
    {
        base.TakeDirectEffectDamage(amount, effect, true, true);

        Vector2 direction = (effect.transform.position - transform.position).normalized;
        if (direction.x > 0)
        {
            GetComponent<Animator>()?.SetBool("IsHurtingRight", true);
        }
        else
        {
            GetComponent<Animator>()?.SetBool("IsHurtingLeft", true);
        }
    }

    public void StopHurting()
    {
        GetComponent<Animator>()?.SetBool("IsHurtingRight", false);
        GetComponent<Animator>()?.SetBool("IsHurtingLeft", false);
    }
}

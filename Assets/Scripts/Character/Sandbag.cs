using UnityEngine;

public class Sandbag : BaseCharacter
{
    public override void TakeDamage(DamageData damageData)
    {
        Vector2 direction = (damageData.SourcePosition - damageData.TargetPosition).normalized;

        if(direction.x >0 )
        {
            GetComponent<Animator>()?.SetBool("IsHurtingRight", true);
        }
        else
        {
            GetComponent<Animator>()?.SetBool("IsHurtingLeft", true);
        }

        base.OnDamageTaken(damageData);
    }

    public void StopHurting()
    {
        GetComponent<Animator>()?.SetBool("IsHurtingRight", false);
        GetComponent<Animator>()?.SetBool("IsHurtingLeft", false);
    }
}

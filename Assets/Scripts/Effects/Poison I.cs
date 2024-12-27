using UnityEngine;

public class PoisonI : Effect
{
    [SerializeField]
    float damagePerTick = 3f;
    [SerializeField]
    float slowPercentage = 0.5f;

    private bool isSlowed = false;
    private float slowAmount = 0f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if(target.Race == RaceType.UNDEAD || target.Race == RaceType.ORC)
        {
            // heal the target instead of damaging it
            target.Heal((int)damagePerTick);
        }
        else
        {
            target?.TakeDirectEffectDamage((int)damagePerTick, this);
            if (!isSlowed)
            {
                slowAmount = target.GetSpeed() * slowPercentage;
                target.ModifySpeed(-slowAmount);
                isSlowed = true;
            }
        }        
    }

    public override void OnEffectEnd()
    {
        if (target.Race != RaceType.UNDEAD && target.Race != RaceType.ORC)
        {
            // revert the slow effect
            target?.ModifySpeed(slowAmount);
        }
    }
}

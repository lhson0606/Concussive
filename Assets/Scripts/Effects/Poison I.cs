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
        target?.TakeDirectEffectDamage((int)damagePerTick, this);
        if(!isSlowed)
        {
            slowAmount = target.GetSpeed() * slowPercentage;
            target.ModifySpeed(-slowAmount);
            isSlowed = true;
        }
    }

    public override void OnEffectEnd()
    {
        // revert the slow effect
        target?.ModifySpeed(slowAmount);
    }
}

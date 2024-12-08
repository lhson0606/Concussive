using UnityEngine;

public class InstantFreezeII : Effect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        animator?.SetTrigger("Freeze");
        target.Freeze();
    }

    public override void OnEffectEnd()
    {
        animator?.SetTrigger("Unfreeze");
        target.Unfreeze();
    }

    // call from animation event
    public void OnDestroyIceEffect()
    {
        base.EndEffect();
    }
}

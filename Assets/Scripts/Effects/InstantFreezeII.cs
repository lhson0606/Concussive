using UnityEngine;

public class InstantFreezeII : Effect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        animator?.SetTrigger("Freeze");
        target.Freeze(base.duration);
    }

    // call from animation event
    public void OnDestroyIceEffect()
    {
        base.EndEffect();
    }
}

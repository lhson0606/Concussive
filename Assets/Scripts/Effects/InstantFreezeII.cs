using UnityEngine;

public class InstantFreezeII : Effect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        animator?.SetTrigger("Freeze");
        target.IsFreezing = true;
    }

    public new void EndEffect()
    {
        animator?.SetTrigger("Unfreeze");
        target.IsFreezing = false;
    }

    // call from animation event
    public void OnDestroyIceEffect()
    {
        base.EndEffect();
    }
}

using UnityEngine;

public class HealingII : Effect
{
    [SerializeField]
    int healPerTick = 1;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        target?.Heal(healPerTick);
    }
}

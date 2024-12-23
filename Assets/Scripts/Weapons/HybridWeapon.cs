using UnityEngine;

public class HybridWeapon : BaseWeapon
{
    [SerializeField]
    protected float SwitchCooldown = 0.4f;

    private float switchCooldownTimer = 0f;

    public enum HybridMode
    {
        Melee,
        Ranged,
    }

    public override void Update()
    {
        base.Update();
        if (switchCooldownTimer > 0)
        {
            switchCooldownTimer -= Time.deltaTime;
        }
    }

    [SerializeField]
    protected HybridMode mode = HybridMode.Melee;

    public override void OnSpecialModeTriggered()
    {
        base.OnSpecialModeTriggered();

        if (switchCooldownTimer > 0)
        {
            return;
        }

        switchCooldownTimer = SwitchCooldown;

        mode = mode == HybridMode.Melee ? HybridMode.Ranged : HybridMode.Melee;
        animator?.SetInteger("HybridState", (int)mode);
    }
}

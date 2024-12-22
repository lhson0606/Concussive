using UnityEngine;

public class LightningStrikeI : Effect
{
    [SerializeField]
    private float damage = 10f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        target?.TakeDirectEffectDamage((int)damage, this);
        PlayerController playerController = GameObject.FindAnyObjectByType<PlayerController>();
        playerController?.ShakePlayerCamera(this.transform.position);
    }
}

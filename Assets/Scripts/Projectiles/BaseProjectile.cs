using UnityEngine;

public class BaseProjectile : SlowMotionObject, IDamageable
{
    protected Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        this.gameObject.tag = damageData.DamageDealer ? damageData.DamageDealer.tag : "Untagged";
        // reverse the arrow direction
        rb.linearVelocity *= -1;
    }

    public void TakeDirectEffectDamage(int amount, Effect effect, bool isInvisible = false)
    {
    }
}

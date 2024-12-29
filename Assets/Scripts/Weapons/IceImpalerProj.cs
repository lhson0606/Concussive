using UnityEngine;

public class IceImpalerProj : BaseProjectile
{
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void OnHit(Collider2D collision)
    {
        base.OnHit(collision);
        animator.SetTrigger("Crash");

        DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource, false);
    }
}

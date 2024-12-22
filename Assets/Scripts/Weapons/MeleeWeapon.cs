using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
    [SerializeField]
    private Collider2D attackCollider;

    public override void DoAttack()
    {
        base.DoAttack();
        //if on cooldown, return
        if (owner.IsAttacking)
        {
            return;
        }

        animator.SetTrigger("Attack");
        owner.IsAttacking = true;

        OnAttackStarted();
    }

    public override void SetAsMainWeapon(BaseCharacter owner)
    {
        base.SetAsMainWeapon(owner);
        base.ShouldAlterRenderOrder = true;
    }

    public void TriggerAttack()
    {
        var hitColliders = new Collider2D[10];
        var count = Physics2D.OverlapCollider(attackCollider, new ContactFilter2D(), hitColliders);

        for (int i = 0; i < count; i++)
        {
            DamageUtils.TryToApplyDamageTo(owner.gameObject, hitColliders[i], damageSource);
        }
    }
}

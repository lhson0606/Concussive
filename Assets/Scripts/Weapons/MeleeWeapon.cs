using System;
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
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true; // Include trigger colliders

        var count = Physics2D.OverlapCollider(attackCollider, contactFilter, hitColliders);

        if (damageSource == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (ReferenceEquals(hitColliders[i].gameObject, owner.gameObject))
            {
                continue;
            }

            DamageUtils.TryToApplyDamageTo(owner?.gameObject, hitColliders[i], damageSource);
        }
    }

    public void SetMeleeOwnerIsAttackingFalse()
    {
        if (owner != null)
        {
            owner.IsAttacking = false;
        }
    }
}

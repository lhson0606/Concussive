using System;
using UnityEngine;

public class SpearScript : HybridWeapon
{
    [SerializeField]
    GameObject spearTip;
    [SerializeField]
    GameObject spearProjectilePrefab;

    private Collider2D spearTipCol;

    protected override void Awake()
    {
        base.Awake();
        spearTipCol = spearTip.GetComponent<Collider2D>();
    }

    public override void OnSpecialModeTriggered()
    {
        base.OnSpecialModeTriggered();
        if (mode == HybridMode.Ranged)
        {
            base.ShouldAlterRenderOrder = false;
            // set render order to character +1
            weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
        }
        else
        {
            base.ShouldAlterRenderOrder = true;
        }
    }

    public override void DoAttack()
    {
        base.DoAttack();
        if (mode == HybridMode.Melee)
        {
            base.ShouldAlterRenderOrder = true;
            MeleeAttack();
        }
        else
        {
            base.ShouldAlterRenderOrder = false;
            // set render order to character +1
            weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
            RangedAttack();
        }
    }

    private void RangedAttack()
    {
        animator?.SetTrigger("Attack");
    }

    private void MeleeAttack()
    {
        animator?.SetTrigger("Attack");
        owner?.ApplyImpulse(transform.up * 10f, 0.05f);
        if (audioSource != null && !audioSource.isPlaying && onAttackSound != null)
        {
            audioSource.PlayOneShot(onAttackSound);
        }
    }

    public void DealMeleeDamage()
    {
        var hitColliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true; // Include trigger colliders
        var count = Physics2D.OverlapCollider(spearTipCol, contactFilter, hitColliders);
        if (damageSource == null)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            DamageUtils.TryToApplyDamageTo(owner?.gameObject, hitColliders[i], damageSource);
        }
    }

    public override void ReleaseAttack()
    {
        base.ReleaseAttack();

        if(mode == HybridMode.Melee)
        {
            return;
        }

        animator?.SetTrigger("Release");
    }

    public void OnProjectileRelease()
    {
        GameObject projectile = Instantiate(spearProjectilePrefab, spearTip.transform.position, spearTip.transform.rotation);
        SpearProjectile spearProjectile = projectile.GetComponent<SpearProjectile>();
        spearProjectile.SetParentWeapon(this);
        spearProjectile.SetDamageSource(damageSource);
        spearProjectile.SetDirection(damageSource.GetDispersedLookDir(transform.up));
        spearProjectile.Launch();
        if (audioSource != null && !audioSource.isPlaying && onAttackSound != null)
        {
            audioSource.PlayOneShot(onAttackSound);
        }
    }
}

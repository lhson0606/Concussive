using System;
using UnityEngine;

public class SpearScript : HybridWeapon
{
    [SerializeField]
    GameObject spearTip;
    [SerializeField]
    GameObject spearProjectilePrefab;
    [SerializeField]
    AudioClip rangedAttackSound;
    [SerializeField]
    AudioClip meleeAttackSound;
    [SerializeField]
    AudioClip rangedHitSound;
    [SerializeField]
    AudioClip rangedHitObstacleSound;

    private Collider2D spearTipCol;

    protected override void Awake()
    {
        base.Awake();
        spearTipCol = spearTip.GetComponent<Collider2D>();
    }

    protected override void Start()
    {
        // we will play our own sounds
        onAttackSound = null;
    }

    public override void OnSpecialModeTriggered()
    {
        base.OnSpecialModeTriggered();
        if (mode == HybridMode.Ranged)
        {
            base.ShouldAlterRenderOrder = false;
            // set render order to character +1
            weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
            damageSource.ResetStats();
            damageSource.Owner = owner.gameObject;
            damageSource.Damage *= 1.5f;
            damageSource.CriticalMultiplier *= 1.25f;
        }
        else
        {
            base.ShouldAlterRenderOrder = true;
            damageSource.ResetStats();
            damageSource.Owner = owner.gameObject;
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

        if(meleeAttackSound && audioSource && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(meleeAttackSound);
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
        if (rangedAttackSound && audioSource && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(rangedAttackSound);
        }
    }

    public AudioClip GetRangedHitSound()
    {
        return rangedHitSound;
    }

    public AudioClip GetRangedHitObstacleSound()
    {
        return rangedHitObstacleSound;
    }
}

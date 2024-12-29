using System;
using UnityEngine;

public class SpearScript : HybridWeapon
{
    [SerializeField]
    private float meleeReloadTime = 0.5f;
    [SerializeField]
    private float rangedReloadTime = 1.2f;
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

    public HybridMode CurrentMode => mode;

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
            damageSource.CoolDown = rangedReloadTime;
        }
        else
        {
            base.ShouldAlterRenderOrder = true;
            damageSource.ResetStats();
            damageSource.Owner = owner.gameObject;
            damageSource.CoolDown = meleeReloadTime;
        }
    }

    public override void DoAttack()
    {
        base.DoAttack();

        if(!damageSource.IsCoolDownReset())
        {
            return;
        }

        if (mode == HybridMode.Melee)
        {
            base.ShouldAlterRenderOrder = true;
            damageSource.ResetStats();
            damageSource.Owner = owner.gameObject;
            damageSource.CoolDown = meleeReloadTime;
            MeleeAttack();
        }
        else
        {
            base.ShouldAlterRenderOrder = false;
            // set render order to character +1
            weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
            damageSource.ResetStats();
            damageSource.Owner = owner.gameObject;
            damageSource.Damage *= 1.5f;
            damageSource.CriticalMultiplier *= 1.25f;
            damageSource.CoolDown = rangedReloadTime;
            RangedAttack();
        }

        damageSource.ApplyCoolDown();
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

    public void OnProjectileRelease()
    {
        GameObject projectile = Instantiate(spearProjectilePrefab, spearTip.transform.position, spearTip.transform.rotation);
        SpearProjectile spearProjectile = projectile.GetComponent<SpearProjectile>();
        spearProjectile.SetAllNecessities(transform.up, this);
        spearProjectile.Launch();
        if (rangedAttackSound && audioSource && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(rangedAttackSound);
        }

        animator?.SetTrigger("Release");
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

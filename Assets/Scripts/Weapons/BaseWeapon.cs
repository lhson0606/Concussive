using System;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(DamageSource))]
public class BaseWeapon : GameItem
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected AudioClip onAttackSound;
    [SerializeField]
    protected AudioClip onEquipSound;
    [SerializeField]
    protected AudioClip onHitSound;
    [SerializeField]
    protected AudioClip onAttackMissSound;

    protected SpriteRenderer weaponSpriteRenderer;

    public readonly int STATE_ON_GROUND = 0;
    public readonly int STATE_IDLE = 1;
    public readonly int STATE_OFF_HAND = 2;

    protected int currentState = 0;
    protected BaseCharacter owner;
    protected AudioSource audioSource;
    private bool _shouldAlterRenderOrder = false;
    public bool ShouldAlterRenderOrder { 
        get => _shouldAlterRenderOrder;
        set
        {
            _shouldAlterRenderOrder = value;
            WeaponControl weaponControl = owner?.GetWeaponControl();
            if (weaponControl != null)
            {
                weaponControl.ShouldAlterRenderOrder = value;
            }
        }
    }
    protected DamageSource damageSource;

    protected override void OnValidate()
    {
        base.OnValidate();
        SetUpPickUp();
    }

    protected override void Awake()
    {
        base.Awake();
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        damageSource = GetComponent<DamageSource>();

        if(damageSource == null)
        {
            Debug.LogError("DamageSource is missing on this weapon!");
        }

        UpdateState(STATE_ON_GROUND);
    }

    public override void DropItem(Vector3 position)
    {
        base.DropItem(position);
        UpdateState(STATE_ON_GROUND);
        damageSource.ResetStats();
    }

    private float GetAnimationClipDuration(string clipName)
    {
        if(!animator)
        {
            return 0f;
        }

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.LogWarning($"Animation clip '{clipName}' not found!");
        return 0f;
    }

    private void UpdateState(int state)
    {
        currentState = state;

        if (animator != null)
        {
            animator.SetInteger("State", currentState);
        }
    }

    protected virtual void OnAttackFinish()
    {

    }

    public void SetOwnerIsAttackingFalse()
    {
        if (owner != null)
        {
            owner.IsAttacking = false;
        }
    }

    public virtual void SetAsMainWeapon(BaseCharacter owner)
    {
        transform.SetParent(owner.GetPrimaryWeaponSlotTransform());
        transform.localPosition = new Vector3(0, 0, 0);
        UpdateState(STATE_IDLE);
        OnEquippedAsMainWeapon();
        SetUpDamageSource(owner);
    }

    public void SetUpAsAutoSecondary(BaseCharacter owner)
    {
        SetOwner(owner);
        UpdateState(STATE_IDLE);
        OnEquippedAsMainWeapon();
        SetUpDamageSource(owner);
    }

    public virtual void SetAsOffHandWeapon(BaseCharacter owner)
    {
        transform.SetParent(owner.GetSecondaryWeaponSlotTransform());
        transform.localPosition = new Vector3(0, 0, 0);
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder - 1;
        UpdateState(STATE_OFF_HAND);
        OnEquippedAsOffHandWeapon();
    }

    public virtual void OnEquippedAsMainWeapon()
    {
    }

    public virtual void OnEquippedAsOffHandWeapon()
    {
    }

    protected override void OnPickUp(BaseCharacter owner)
    {
        Debug.Log("Weapon picked up");
        owner.EquipWeapon(this, owner.GetCharacterSpriteRenderer(), GetWeaponSpriteRenderer());
        this.owner = owner;
        SetUpDamageSource(owner);
    }

    private void SetUpDamageSource(BaseCharacter owner)
    {
        damageSource.ResetStats();
        damageSource.Owner = owner.gameObject;
        // #todo: set damage, critical chance, critical multiplier, element, damage type, etc.
    }

    public DamageData ApplyCritableDamageBuff(DamageData damageData, BaseCharacter owner, BaseCharacter target)
    {
        //apply critable damage buffs here
        return damageData;
    }

    private DamageData ApplyNoncritableDamageBuff(DamageData damageData, BaseCharacter owner, BaseCharacter target)
    {
        //apply non-critable damage buffs here
        return damageData;
    }

    public virtual void DoAttack()
    {
        OnAttackStarted();
    }


    public virtual void ReleaseAttack()
    {

    }

    public virtual void Update()
    {
        
    }

    public SpriteRenderer GetWeaponSpriteRenderer()
    {
        return weaponSpriteRenderer;
    }

    public bool IsAttacking()
    {
        return owner.IsAttacking;
    }

    protected virtual void OnHit()
    {
        PlayOnHitSound();
    }

    public void OnEquipped()
    {
        if(owner.tag == "Player")
        {
            PlayEquipSound();
        }
    }

    protected virtual void OnAttackStarted()
    {
        PlayOnAttackSound();
    }

    protected void OnAttackMissed()
    {
        PlayOnAttackMissSound();
    }

    public void PlayOnHitSound()
    {
        if (onHitSound != null)
        {
            audioSource.PlayOneShot(onHitSound);
        }
    }

    public void PlayEquipSound()
    {
        if (onEquipSound != null)
        {
            audioSource.PlayOneShot(onEquipSound);
        }
    }

    public void PlayOnAttackMissSound()
    {
        if (onAttackMissSound != null)
        {
            audioSource.PlayOneShot(onAttackMissSound);
        }
    }

    public void PlayOnAttackSound()
    {
        if (audioSource && !audioSource.isPlaying && onAttackSound != null)
        {
            audioSource.PlayOneShot(onAttackSound);
        }
    }

    internal void SetOwner(BaseCharacter baseCharacter)
    {
        this.owner = baseCharacter;
    }

    internal BaseCharacter GetOwner()
    {
        return owner;
    }

    public DamageSource GetDamageSource()
    {
        return damageSource;
    }

    public virtual void OnSpecialModeTriggered()
    {
    }

    internal AudioClip GetOnHitSound()
    {
        return onHitSound;
    }
}

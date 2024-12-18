using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class BaseWeapon : GameItem
{
    [SerializeField]
    protected int baseDamage = 0;
    [SerializeField]
    protected float weaponCriticalChance = 0.05f;
    [SerializeField]
    protected float attackSpeed = 1.0f;
    [SerializeField]
    protected Element element = null;
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
    public bool ShouldAlterRenderOrder { get; set; } = true;

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
        attackSpeed = 1f / GetAnimationClipDuration("WeaponAttackAnim");
        audioSource = GetComponent<AudioSource>();
        updateState(STATE_ON_GROUND);
    }

    public override void DropItem(Vector3 position)
    {
        base.DropItem(position);
        updateState(STATE_ON_GROUND);
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

    private void updateState(int state)
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

    private void SetOwnerIsAttackingFalse()
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
        updateState(STATE_IDLE);
        OnEquippedAsOffHandWeapon();
    }

    public virtual void SetAsOffHandWeapon(BaseCharacter owner)
    {
        transform.SetParent(owner.GetSecondaryWeaponSlotTransform());
        transform.localPosition = new Vector3(0, 0, 0);
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder - 1;
        updateState(STATE_OFF_HAND);
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
    }

    public DamageData CalculateDamage(BaseCharacter owner, BaseCharacter target)
    {
        DamageData damageData = new DamageData();
        damageData.Damage = baseDamage;
        damageData.IsCritical = IsCriticalHit(owner);
        damageData.SourceElement = element;
        damageData.SourcePosition = owner.transform.position;
        damageData.TargetPosition = target.transform.position;

        // Apply critable damage buffs
        damageData = ApplyCritableDamageBuff(damageData, owner, target);

        if (element == null)
        {
            return damageData;
        }

        // Calculate crit damage
        if (damageData.IsCritical)
        {
            if(element.IsElemental)
            {
                // apply elemental effect
                Effect effectPrefab = element.Effect;
                if (effectPrefab != null)
                {
                    Effect effectInstance = Instantiate(effectPrefab, target.transform.position, Quaternion.identity, target.transform);
                    effectInstance.StartEffect(target);
                }
            }
            else
            {
                damageData.Damage *= owner.GetCriticalDamageMultiplier();
            }
        }

        return damageData;
    }

    private DamageData ApplyCritableDamageBuff(DamageData damageData, BaseCharacter owner, BaseCharacter target)
    {
        //apply critable damage buffs here
        return damageData;
    }

    private DamageData ApplyNoncritableDamageBuff(DamageData damageData, BaseCharacter owner, BaseCharacter target)
    {
        //apply non-critable damage buffs here
        return damageData;
    }

    private bool IsCriticalHit(BaseCharacter owner)
    {
        if(owner == null)
        {
            return Random.value < this.weaponCriticalChance;
        }
        else return Random.value < owner.GetCriticalChance() + this.weaponCriticalChance;
    }

    public virtual void DoAttack()
    {
        
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
        if (onAttackSound != null)
        {
            audioSource.PlayOneShot(onAttackSound);
        }
    }

    internal void SetOwner(BaseCharacter baseCharacter)
    {
        this.owner = baseCharacter;
    }
}

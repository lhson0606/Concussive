using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
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

    protected SpriteRenderer weaponSpriteRenderer;

    public readonly int STATE_ON_GROUND = 0;
    public readonly int STATE_IDLE = 1;
    public readonly int STATE_OFF_HAND = 2;

    protected int currentState = 0;
    protected BaseCharacter owner;

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
        updateState(STATE_ON_GROUND);
    }

    private float GetAnimationClipDuration(string clipName)
    {
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
        animator.SetInteger("State", currentState);
    }

    private void SetOwnerIsAttackingFalse()
    {
        if (owner != null)
        {
            owner.IsAttacking = false;
        }
    }

    public void SetAsMainWeapon(BaseCharacter owner)
    {
        transform.SetParent(owner.GetPrimaryWeaponSlotTransform());
        transform.localPosition = new Vector3(0, 0, 0);
        updateState(STATE_IDLE);
    }

    public void SetAsOffHandWeapon(BaseCharacter owner)
    {
        transform.SetParent(owner.GetSecondaryWeaponSlotTransform());
        transform.localPosition = new Vector3(0,0,0);
        updateState(STATE_OFF_HAND);
    }

    protected override void OnPickUp(BaseCharacter owner)
    {
        Debug.Log("Weapon picked up");
        owner.EquipWeapon(this, owner.GetCharacterSpriteRenderer(), GetWeaponSpriteRenderer());
        this.owner = owner;
    }

    protected DamageData CalculateDamage(BaseCharacter owner, BaseCharacter target)
    {
        DamageData damageData = new DamageData();
        damageData.Damage = baseDamage;
        damageData.IsCritical = IsCriticalHit(owner);
        damageData.SourceElement = element;

        // Apply critable damage buffs
        damageData = ApplyCritableDamageBuff(damageData, owner, target);

        if(element == null)
        {
            return damageData;
        }

        // Calculate crit damage
        if (damageData.IsCritical && !element.IsElemental)
        {
            damageData.Damage *= owner.GetCriticalDamageMultiplier();
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
        return Random.value < owner.GetCriticalChance() + this.weaponCriticalChance;
    }

    public void DoAttack()
    {
        //if on cooldown, return
        if (owner.IsAttacking)
        {
            return;
        }

        animator.SetTrigger("Attack");
        owner.IsAttacking = true;
    }

    public SpriteRenderer GetWeaponSpriteRenderer()
    {
        return weaponSpriteRenderer;
    }

    public bool IsAttacking()
    {
        return owner.IsAttacking;
    }
}

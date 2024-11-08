using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseWeapon : GameItem
{
    [SerializeField]
    protected int baseDamage = 0;
    [SerializeField]
    protected float attackSpeed = 1.0f;
    [SerializeField]
    protected Element element = null;
    [SerializeField]
    protected Animator animator;

    protected SpriteRenderer weaponSpriteRenderer;
    private bool isAttacking = false;

    public readonly int STATE_ON_GROUND = 0;
    public readonly int STATE_IDLE = 1;
    public readonly int STATE_OFF_HAND = 2;

    private int currentState = 0;

    protected override void OnValidate()
    {
        base.OnValidate();
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        SetUpPickUp();
        updateState(STATE_ON_GROUND);
    }


    protected override void Start()
    {
        base.Start();        
        updateState(STATE_ON_GROUND);
    }

    private void updateState(int state)
    {
        currentState = state;
        animator.SetInteger("State", currentState);
    }

    public void SetAsMainWeapon(Transform primaryWeaponSlotTransform)
    {
        transform.SetParent(primaryWeaponSlotTransform);
        transform.localPosition = new Vector3(0, 0, 0);
        updateState(STATE_IDLE);
    }

    public void SetAsOffHandWeapon(Transform secondaryWeaponSlotTransform)
    {
        transform.SetParent(secondaryWeaponSlotTransform);
        transform.localPosition = new Vector3(0,0,0);
        updateState(STATE_OFF_HAND);
    }

    protected override void OnPickUp(BaseCharacter owner)
    {
        Debug.Log("Weapon picked up");
        owner.EquipWeapon(this, owner.GetCharacterSpriteRenderer(), GetWeaponSpriteRenderer());
    }

    private DamageData CalculateDamage(BaseCharacter owner, BaseCharacter target)
    {
        DamageData damageData = new DamageData();
        damageData.Damage = baseDamage;
        damageData.IsCritical = IsCriticalHit(owner);
        damageData.SourceElement = element;

        // Apply critable damage buffs
        damageData = ApplyCritableDamageBuff(damageData, owner, target);

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
        return Random.value < owner.GetCriticalChance();
    }

    public void DoAttack()
    {
        //if on cooldown, return
        if (isAttacking)
        {
            return;
        }

        animator.SetTrigger("Attack");
        isAttacking = true;
        //create an invoke to reset the attack
        Invoke("ResetAttack", 1 / attackSpeed);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public SpriteRenderer GetWeaponSpriteRenderer()
    {
        return weaponSpriteRenderer;
    }
}

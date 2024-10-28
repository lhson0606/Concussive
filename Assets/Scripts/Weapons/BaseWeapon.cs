using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameItem))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseWeapon : SlowMotionObject
{
    [SerializeField]
    protected int baseDamage = 0;
    [SerializeField]
    protected float attackSpeed = 1.0f;
    [SerializeField]
    protected Element element = null;
    [SerializeField]
    protected Animator animator = null;
    [SerializeField]
    protected Transform mainWeaponTransform;
    [SerializeField]
    protected Transform offHandWeaponTransform;


    protected PickUpComponent pickUpComponent;
    protected SpriteRenderer weaponSpriteRenderer;
    private WeaponState weaponState = WeaponState.ON_GROUND;
    private GameItem gameItem;

    protected void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        gameItem = GetComponent<GameItem>();

        UpdateWeaponState();
        SetUpPickUp();
    }

    private void SetWeaponState(WeaponState state)
    {
        weaponState = state;
        UpdateWeaponState();
    }

    public void SetAsMainWeapon(Transform primaryWeaponSlotTransform)
    {
        transform.localPosition = primaryWeaponSlotTransform.localPosition;
        transform.localRotation = primaryWeaponSlotTransform.localRotation;
        SetWeaponState(WeaponState.MAIN_WEAPON);
    }

    public void SetAsOffHandWeapon(Transform secondaryWeaponSlotTransform)
    {
        transform.localPosition = secondaryWeaponSlotTransform.localPosition;
        transform.localRotation = secondaryWeaponSlotTransform.localRotation;
        SetWeaponState(WeaponState.OFF_HAND);
    }

    private void UpdateWeaponState()
    {
        switch (weaponState)
        {
            case WeaponState.ON_GROUND:
                transform.localPosition += gameItem.GetOnGroundTransform().localPosition;
                transform.localRotation = gameItem.GetOnGroundTransform().localRotation;
                transform.localScale = gameItem.GetOnGroundTransform().localScale;
                break;
            case WeaponState.MAIN_WEAPON:
                transform.localPosition += mainWeaponTransform.localPosition;
                transform.localRotation = mainWeaponTransform.localRotation;
                transform.localScale = mainWeaponTransform.localScale;
                break;
            case WeaponState.OFF_HAND:
                transform.localPosition += offHandWeaponTransform.localPosition;
                transform.localRotation = offHandWeaponTransform.localRotation;
                transform.localScale = offHandWeaponTransform.localScale;
                break;
        }
    }

    private void SetUpPickUp()
    {
        pickUpComponent = GetComponent<PickUpComponent>();
        if (pickUpComponent != null)
        {
            pickUpComponent.OnPickedUp += OnEquip; // Adjusted to match Action delegate
        }
    }

    private void OnEquip(BaseCharacter owner)
    {
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

    }

    public SpriteRenderer GetWeaponSpriteRenderer()
    {
        return weaponSpriteRenderer;
    }
}

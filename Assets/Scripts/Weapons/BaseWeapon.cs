using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameItem))]
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
    protected BaseCharacter owner = null;

    private void OnEquip(BaseCharacter owner)
    {
        //gameObject.Se
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
}

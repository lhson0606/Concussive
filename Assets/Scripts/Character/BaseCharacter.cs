using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class BaseCharacter : SlowMotionObject, IDamageable
{
    [SerializeField]
    protected int maxHealth = 7;
    [SerializeField]
    protected int maxArmor = 3;
    [SerializeField]
    protected int maxMana = 200;
    [SerializeField]
    protected float runSpeed = 240.0f;
    [SerializeField]
    protected float criticalHitChance = 0.05f;
    [SerializeField]
    protected float criticalHitMultiplier = 2.0f;
    [SerializeField]
    protected float bareHandDamage = 1.0f;
    [SerializeField]
    protected float bareHandAttackSpeed = 1.0f;
    [SerializeField]
    protected GameObject characterText = null;
    [SerializeField]
    protected RaceType race = RaceType.HUMAN;
    [SerializeField]
    protected AudioClip hurtSound = null;
    [SerializeField]
    protected float effectSizeScale = 1f;

    protected List<Effect> effects = new List<Effect>();
    protected Dictionary<BuffType, List<Buff>> buffs = new Dictionary<BuffType, List<Buff>>();

    [SerializeField] protected int currentHealth;
    protected int currentArmor;
    protected int currentMana;
    protected GameObject primaryWeaponSlot;
    protected GameObject secondaryWeaponSlot;
    protected BaseWeapon primaryWeapon;
    protected BaseWeapon secondaryWeapon; // Secondary weapon can be null

    protected WeaponControl weaponControl;
    protected SpriteRenderer characterRenderer;
    protected AudioSource audioSource;

    protected SimpleFlashEffect flashEffect;

    public Vector2 LookDir { get; private set; }
    public Vector2 LookAtPosition { get; set; }

    public bool IsAttacking { get; set; } = false;
    public bool IsMovementEnabled { get; set; } = true;
    public bool IsFreezing { get; set; } = false;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public BaseWeapon PrimaryWeapon
    {
        get { return primaryWeapon; }
    }

    // Define the WeaponEquipHandler event
    public event Action<BaseWeapon> WeaponEquipHandler;

    public virtual void Start()
    {
        // Log the tag of the GameObject
        Debug.Log("BaseCharacter Start - Tag: " + gameObject.tag);

        currentHealth = maxHealth;
        currentArmor = maxArmor;
        currentMana = maxMana;
        primaryWeaponSlot = transform.Find("PrimaryWeapon")?.gameObject;
        secondaryWeaponSlot = transform.Find("SecondaryWeapon")?.gameObject;

        characterRenderer = GetComponent<SpriteRenderer>();
        primaryWeapon = primaryWeaponSlot?.GetComponent<BaseWeapon>();
        weaponControl = primaryWeaponSlot?.GetComponent<WeaponControl>();
        audioSource = GetComponent<AudioSource>();
        LookAtPosition = transform.position;
        LookDir = Vector2.right;

        flashEffect = GetComponent<SimpleFlashEffect>();
    }

    public void EnableMovement()
    {
        IsMovementEnabled = true;
    }

    public void DisableMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        IsMovementEnabled = false;
    }

    public virtual void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        if (IsAttacking)
        {
            return;
        }

        LookDir = (LookAtPosition - (Vector2)transform.position).normalized;

        if (LookDir.x < 0)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (LookDir.x > 0)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (weaponControl != null)
        {
            weaponControl.PointerPosition = LookAtPosition;
        }
    }

    public virtual void Die()
    {
        Debug.Log("Character died.");
    }

    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
    }

    public void RemoveEffect(Effect effect)
    {
        effects.Remove(effect);
    }

    public void Heal(int healingAmount, EffectType healingType = EffectType.HEALING)
    {
        Debug.Log("Healing " + healingAmount + " health");
        if (currentHealth + healingAmount > maxHealth)
        {
            healingAmount = 0;
        }

        currentHealth += healingAmount;

        String text = "+" + healingAmount + " HP";
        Color textColor = EffectConfig.Instance.GetEffectTextColor(healingType);
        float lifeTime = 0.8f;
        Vector2 initVel = new Vector2(0, 1);
        this.SpawnText(text, textColor, lifeTime, initVel);

    }

    public void SpawnDamageText(DamageData damageData)
    {
        if (characterText == null)
        {
            Debug.LogError("CharacterText prefab is not assigned.");
            return;
        }

        var color = Color.white;
        var text = damageData.Damage.ToString();
        float lifeTime = 0.8f;
        Vector2 initVel = new Vector2(0, 1);
        float scale = 1;

        if (damageData.IsCritical)
        {
            color = damageData.SourceElement.IsElemental
                ? EffectConfig.Instance.GetEffectTextColor(damageData.SourceElement.Effect.EffectType)
                : Color.red;
            initVel *= GetCriticalDamageMultiplier();
            scale = 1.5f;
        }
        else if (damageData.SourceElement.IsElemental)
        {
            color = EffectConfig.Instance.GetEffectTextColor(damageData.SourceElement.Effect.EffectType);
        }

        this.SpawnText(text, color, lifeTime, initVel, scale);
    }

    public void SpawnText(string text, Color color, float lifeTime, Vector2 initVel, float scale = 1)
    {
        if (characterText == null)
        {
            Debug.LogError("CharacterText prefab is not assigned.");
            return;
        }

        GameObject textObject = Instantiate(characterText, transform.position, Quaternion.identity);
        PopUpTextControl textControl = textObject.GetComponent<PopUpTextControl>();

        if (textControl == null)
        {
            Debug.LogError("PopUpTextControl component not found on characterText prefab.");
            return;
        }

        textControl.text = text;
        textControl.textColor = color;
        textControl.lifeTime = lifeTime;
        textControl.InitialVelocity = initVel;
        textControl.transform.localScale *= scale;
    }

    public void AddBuff(Buff buff)
    {
        if (buffs.ContainsKey(buff.type))
        {
            if (buff.canStack)
            {
                buffs[buff.type].Add(buff);
            }
            else
            {
                buffs[buff.type][0].value = Math.Max(buffs[buff.type][0].value, buff.value);
            }
        }
        else
        {
            List<Buff> buffList = new List<Buff>();
            buffList.Add(buff);
            buffs.Add(buff.type, buffList);
        }
    }

    public void RemoveBuff(Buff buff)
    {
        if (buffs.ContainsKey(buff.type))
        {
            if (buffs[buff.type].Count > 0)
            {
                buffs[buff.type].Remove(buff);
            }

            if (buffs[buff.type].Count == 0)
            {
                buffs.Remove(buff.type);
            }
        }
    }

    public bool HasBuff(BuffType buffType)
    {
        return buffs.ContainsKey(buffType);
    }

    public List<Buff> GetBuff(BuffType buffType)
    {
        if (buffs.ContainsKey(buffType))
        {
            return buffs[buffType];
        }

        return null;
    }

    public float GetCriticalChance()
    {
        float result = criticalHitChance;
        List<Buff> criticalChanceBuffList = GetBuff(BuffType.INCREASE_CRITICAL_CHANCE);

        if (criticalChanceBuffList != null)
        {
            foreach (Buff buff in criticalChanceBuffList)
            {
                result += buff.value;
            }
        }

        return result;
    }

    public float GetCriticalDamageMultiplier()
    {
        float result = criticalHitMultiplier;
        List<Buff> buffs = GetBuff(BuffType.INCREASE_CRITICAL_DAMAGE);

        if (buffs != null)
        {
            foreach (Buff buff in buffs)
            {
                result += buff.value;
            }
        }

        return result;
    }

    public float GetBareHandDamage()
    {
        return bareHandDamage;
    }

    public float GetRunSpeed()
    {
        return runSpeed;
    }

    public virtual void EquipWeapon(BaseWeapon weapon, SpriteRenderer characterRenderer, SpriteRenderer weaponRenderer)
    {
        if (weaponControl == null)
        {
            throw new ArgumentException("Weapon control is not assigned.");
        }

        if (primaryWeapon == null)
        {
            primaryWeapon = weapon;
            weapon.SetAsMainWeapon(this);
            weaponControl.characterRenderer = characterRenderer;
            weaponControl.weaponRenderer = weaponRenderer;
            weaponControl.ShouldAlterRenderOrder = weapon.ShouldAlterRenderOrder;
            primaryWeapon.OnEquipped();
        }
        else if (secondaryWeapon == null)
        {
            secondaryWeapon = weapon;
            weapon.SetAsOffHandWeapon(this);
        }
        else
        {
            primaryWeapon.DropItem(weapon.transform.position);
            primaryWeapon = weapon;
            weaponControl.weaponRenderer = primaryWeapon.GetComponent<SpriteRenderer>();
            weaponControl.ShouldAlterRenderOrder = weapon.ShouldAlterRenderOrder;
            primaryWeapon.SetAsMainWeapon(this);
            primaryWeapon.OnEquipped();
        }

        // Call the OnEquipWeapon method
        OnEquipWeapon(weapon);
    }

    // New virtual method to handle weapon equip event
    protected virtual void OnEquipWeapon(BaseWeapon weapon)
    {
        Debug.Log("Weapon equipped: " + weapon.name);

        // Trigger the WeaponEquipHandler event
        WeaponEquipHandler?.Invoke(weapon);
    }

    public void SwitchToSecondaryWeapon()
    {
        if (secondaryWeapon == null)
        {
            Debug.LogWarning("No secondary weapon equipped.");
            return;
        }

        BaseWeapon tempWeapon = primaryWeapon;
        primaryWeapon = secondaryWeapon;
        secondaryWeapon = tempWeapon;

        weaponControl.weaponRenderer = primaryWeapon.GetComponent<SpriteRenderer>();
        primaryWeapon.SetAsMainWeapon(this);
        secondaryWeapon.SetAsOffHandWeapon(this);
        weaponControl.ShouldAlterRenderOrder = primaryWeapon.ShouldAlterRenderOrder;

        primaryWeapon.OnEquipped();

        Debug.Log("Switched to secondary weapon.");
    }

    public void SetWeaponPointer(Vector2 val)
    {
        weaponControl.PointerPosition = val;
    }

    public SpriteRenderer GetCharacterSpriteRenderer()
    {
        return characterRenderer;
    }

    public BaseWeapon GetPrimaryWeapon()
    {
        return primaryWeapon;
    }

    public Transform GetPrimaryWeaponSlotTransform()
    {
        return primaryWeaponSlot?.transform;
    }

    public Transform GetSecondaryWeaponSlotTransform()
    {
        return secondaryWeaponSlot?.transform;
    }

    protected virtual void OnDamageTaken(DamageData damageData)
    {
        flashEffect?.Flash();

        SpawnDamageText(damageData);

        if (hurtSound)
        {
            audioSource?.PlayOneShot(hurtSound);
        }
    }

    public virtual void TakeDamage(DamageData damageData)
    {
        Debug.Log("Taking damage");
        Debug.Log("Is critical hit: " + damageData.IsCritical);

        Debug.Log("Damage: " + damageData.Damage);

        currentHealth = currentHealth - (int)damageData.Damage;

        // Add impulse to the character
        Vector2 dir = damageData.TargetPosition - damageData.SourcePosition;
        Vector2 impulse = dir.normalized * 10;
        if (damageData.IsCritical)
        {
            impulse *= GetCriticalDamageMultiplier();
        }

        OnDamageTaken(damageData);
        
        if(IsMovementEnabled)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                DisableMovement();
                rb.velocity += impulse;
                StartCoroutine(KnockCo());
            }           
            
        }        
    }

    private IEnumerator KnockCo()
    {
        yield return new WaitForSeconds(0.1f);
        EnableMovement();
    }

    public float GetEffectSizeScale()
    {
        return effectSizeScale;
    }

    public bool CanMove()
    {
        return IsMovementEnabled && !IsFreezing;
    }

    internal void Freeze()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        IsFreezing = true;

        Animator animator = GetComponent<Animator>();
        
        if(animator != null)
        {
            animator.speed = 0;
        }
    }

    internal void Unfreeze()
    {
        IsFreezing = false;
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.speed = 1;
        }
    }
}

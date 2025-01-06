using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseCharacter : SlowMotionObject, IDamageable, IControlButtonInteractable
{
    [SerializeField]
    protected int maxHealth = 7;
    [SerializeField]
    protected int maxArmor = 3;
    [SerializeField]
    protected int maxMana = 200;
    [SerializeField]
    protected float runSpeed = 2.4f;
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
    protected AudioClip idleSound = null;
    [SerializeField]
    protected AudioClip deathSound = null;
    [SerializeField]
    protected float effectSizeScale = 1f;
    [SerializeField]
    protected List<GameObject> dropOnDeath = new List<GameObject>();
    [SerializeField]
    protected GameObject initialPrimaryWeapon = null;
    [SerializeField]
    protected GameObject initialSecondaryWeapon = null;
    [SerializeField]
    protected bool pushable = true;
    [SerializeField]
    [Range(0, 1)]
    protected float pushScale = 1f;
    [SerializeField]
    private bool canUseArmor = false;
    
    private float forgetTakingDamageDelay = 5f;

    protected List<Effect> effects = new List<Effect>();
    protected List<EffectType> effectTypes = new List<EffectType>();

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
    protected Animator animator = null;
    protected Rigidbody2D rb = null;
    private NavMeshAgent navMeshAgent;
    private Coroutine impulseCoroutine = null;
    private Coroutine armorRecoveryCoroutine = null;
    private Coroutine forgetTakingDamageCoroutine = null;
    private bool isTakingDamage = false;

    protected SimpleFlashEffect flashEffect;

    public Vector2 LookDir 
    { 
        get
        {
            return LookAtPosition - (Vector2)transform.position;
        } 
    }

    public Vector2 LookAtPosition { get; set; }

    public bool IsAttacking { get; set; } = false;
    public bool IsMovementEnabled { get; set; } = true;
    private float freezeTimeLeft = 0f;
    public bool IsFreezing { get; set; } = false;
    public bool IsDead { get; set; } = false;

    protected bool isHurt = false;

    // delegates
    protected event Action<DamageData> OnHurt;
    public event Action OnDeath;
    protected event Action<bool> OnCanMoveStateChanged;
    protected event Action<float> OnSpeedChange;

    public void SetIsHurtTrue()
    {
        isHurt = true;
        animator?.SetBool("IsHurt", isHurt);
    }

    public void SetIsHurtFalse()
    {
        isHurt = false;
        animator?.SetBool("IsHurt", isHurt);
    }

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
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        currentMana = maxMana;
        primaryWeaponSlot = transform.Find("PrimaryWeapon")?.gameObject;
        secondaryWeaponSlot = transform.Find("SecondaryWeapon")?.gameObject;

        characterRenderer = GetComponent<SpriteRenderer>();
        primaryWeapon = primaryWeaponSlot?.GetComponent<BaseWeapon>();
        weaponControl = primaryWeaponSlot?.GetComponent<WeaponControl>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        LookAtPosition = transform.position;
        // LookDir is set to right by default
        LookAtPosition = (Vector2)transform.position + Vector2.right;

        flashEffect = GetComponent<SimpleFlashEffect>();

        rb.freezeRotation = true;


        if (initialSecondaryWeapon != null)
        {
            BaseWeapon weapon = Instantiate(initialSecondaryWeapon, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<BaseWeapon>();
            weapon.SetOwner(this);
            EquipWeapon(weapon, characterRenderer, weapon.GetWeaponSpriteRenderer());
        }

        if (initialPrimaryWeapon != null)
        {
            BaseWeapon weapon = Instantiate(initialPrimaryWeapon, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<BaseWeapon>();
            weapon.SetOwner(this);
            EquipWeapon(weapon, characterRenderer, weapon.GetWeaponSpriteRenderer());
        }
    }

    public void EnableMovement()
    {
        IsMovementEnabled = true;
        OnCanMoveStateChanged?.Invoke(CanMove());
    }

    public void DisableMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        IsMovementEnabled = false;
        OnCanMoveStateChanged?.Invoke(CanMove());
    }

    public virtual void Update()
    {
        if(IsFreezing || !CanMove())
        {
            return;
        }

        if (IsAttacking)
        {
            return;
        }

        Vector2 currentLookDir = LookDir;
        currentLookDir = (LookAtPosition - (Vector2)transform.position).normalized;

        if (currentLookDir.x < 0)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (currentLookDir.x > 0)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (weaponControl != null)
        {
            weaponControl.PointerPosition = LookAtPosition;
        }
        UpdateMovingAnimation();
        CheckArmorRecovery();
    }

    private void CheckArmorRecovery()
    {
        if(canUseArmor && currentArmor < maxArmor)
        {
            if(isTakingDamage && forgetTakingDamageCoroutine == null)
            {
                forgetTakingDamageCoroutine = StartCoroutine(ForgetTakingDamage());
            }
            else if(!isTakingDamage && armorRecoveryCoroutine == null)
            {
                armorRecoveryCoroutine = StartCoroutine(RecoverArmor());
            }
        }
    }

    private IEnumerator RecoverArmor()
    {
        yield return new WaitForSeconds(0.5f);
        currentArmor = Math.Min(maxArmor, currentArmor + 1);
        armorRecoveryCoroutine = null;
    }

    private IEnumerator ForgetTakingDamage()
    {
        if(isTakingDamage == false || forgetTakingDamageCoroutine!=null)
        {
            forgetTakingDamageCoroutine = null;
            yield break;
        }
        //delay for forgetTakingDamageDelay
        yield return new WaitForSeconds(forgetTakingDamageDelay);
        isTakingDamage = false;
        forgetTakingDamageCoroutine = null;
    }

    public virtual void UpdateMovingAnimation()
    {
        Vector2 moveVector = rb.linearVelocity;
        animator?.SetBool("IsMoving", moveVector.magnitude > 0);
    }

    public virtual void Die()
    {
        if(IsDead)
        {
            return;
        }

        IsDead = true;

        foreach (GameObject drop in dropOnDeath)
        {
            if (drop != null)
            {
                GameObject dropInstance = Instantiate(drop, transform.position, Quaternion.identity);
                GameItem gameItem = dropInstance.GetComponent<GameItem>();

                if(gameItem != null)
                {
                    Vector2 dropPosition = transform.position;
                    gameItem.DropItem(dropPosition);
                }
            }
        }
        OnDeath?.Invoke();
        // Play the death sound at the current position
        if(deathSound != null)
        {
            AudioUtils.PlayAudioClipAtPoint(deathSound, transform.position);
        }

        if (gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
            return;
        }

        // make the character invisible and schedule it for destruction
        Collider2D col = GetComponent<Collider2D>();
        if(col != null)
        {
            col.enabled = false;
        }
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.2f);
    }

    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
        effectTypes.Add(effect.EffectType);
        SpawnEffectText(effect);
    }

    public void RemoveEffect(Effect effect)
    {
        effects.Remove(effect);
        effectTypes.Remove(effect.EffectType);
    }

    public bool HasEffect(Effect effect)
    {
        return effectTypes.Contains(effect.effectType);
    }

    public Effect GetFirstEffectByType(EffectType effectType)
    {
        foreach (Effect effect in effects)
        {
            if (effect.EffectType == effectType)
            {
                return effect;
            }
        }
        return null;
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

    //public void SpawnDamageText(DamageData damageData)
    //{
    //    if (characterText == null)
    //    {
    //        return;
    //    }

    //    var color = Color.white;
    //    var text = ((int)damageData.Damage).ToString();
    //    float lifeTime = 0.8f;
    //    Vector2 initVel = new Vector2(0, 1);
    //    float scale = 1;

    //    if (damageData.SourceElement.IsElemental)
    //    {
    //        color = EffectConfig.Instance.GetEffectTextColor(damageData.SourceElement.Effect.EffectType);
    //    }

    //    if (damageData.IsCritical)
    //    {
    //        if(damageData.SourceElement.IsElemental)
    //        {
    //            String effectName = damageData.SourceElement.Effect.effectName;
    //            // Spawn the effect name
    //            this.SpawnText(effectName, color, lifeTime, initVel, scale);
    //        }
    //        else
    //        {
    //            initVel *= 1.5f;
    //            scale = 1.5f;
    //            color = Color.red;
    //        }
    //    }

    //    this.SpawnText(text, color, lifeTime, initVel, scale);
    //}

    public void SpawnText(string text, Color color, float lifeTime, Vector2 initVel, float scale = 1)
    {
        if (characterText == null)
        {
            return;
        }

        GameObject textObject = Instantiate(characterText, transform.position, Quaternion.identity);
        PopUpTextControl textControl = textObject.GetComponent<PopUpTextControl>();

        if (textControl == null)
        {
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
            // Call the OnEquipWeapon method
            OnEquipWeapon(weapon);
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
        }
    }

    // New virtual method to handle weapon equip event
    protected virtual void OnEquipWeapon(BaseWeapon weapon)
    {
        // Trigger the WeaponEquipHandler event
        WeaponEquipHandler?.Invoke(weapon);
    }

    public void SwitchToSecondaryWeapon()
    {
        if (secondaryWeapon == null)
        {
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
    public BaseWeapon GetSecondaryWeapon()
    {
        return secondaryWeapon;
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

        // SpawnDamageText(damageData);

        if (hurtSound && !audioSource.isPlaying)
        {
            audioSource?.PlayOneShot(hurtSound);
        }

        SetIsHurtTrue();
        OnHurt?.Invoke(damageData);
    }

    public virtual void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        if (!isInvisible)
        {
            ApplyDamageWithArmor((int)damageData.Damage, damageData.IsCritical);
        }

        //if(damageData.IsCritical && damageData.SourceElement.IsElemental)
        //{
        //    SpawnEffectText(damageData.SourceElement.Effect);
        //}

        // Add impulse to the character
        if (pushable && damageData.PushScale > 0 && !isInvisible)
        {
            Vector2 dir = damageData.TargetPosition - damageData.SourcePosition;
            const float pushForce = 8f;
            Vector2 impulse = dir.normalized * damageData.PushScale * pushForce;
            if (damageData.IsCritical)
            {
                impulse *= GetCriticalDamageMultiplier();
            }

            if (IsMovementEnabled)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                if (rb != null && damageData.PushScale > 0)
                {
                    DisableMovement();
                    rb.linearVelocity += impulse * pushScale;
                    StartCoroutine(KnockCo());
                }

            }
        }        

        OnDamageTaken(damageData);         
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void SpawnDamageText(int healthLoss, int armorLoss, bool isCritical)
    {
        if (characterText == null)
        {
            return;
        }
        var color = Color.white;
        var text = "";
        float lifeTime = 0.8f;
        Vector2 initVel = new Vector2(0, 1);
        float scale = 1;
        if (isCritical)
        {
            initVel *= 1.5f;
            scale = 1.15f;
            color = Color.red;
        }
        if (healthLoss > 0)
        {
            color = Color.red;
            text = "-" + healthLoss + " HP";
            this.SpawnText(text, color, lifeTime, initVel, scale*1.15f);
        }
        if (armorLoss > 0)
        {
            color = Color.gray;
            text = "-" + armorLoss + " Armor";
            this.SpawnText(text, color, lifeTime, initVel, scale);
        }
    }

    private void SpawnEffectText(Effect effect)
    {
        if(characterText == null)
        {
            return;
        }

        var color = Color.white;
        var text = "";
        float lifeTime = 1f;
        Vector2 initVel = new Vector2(0, 1);
        float scale = 1.15f;

        if (effect != null)
        {
            text = effect.effectName;
            color = EffectConfig.Instance.GetEffectTextColor(effect.effectType);
            this.SpawnText(text, color, lifeTime, initVel, scale);
        }
    }

    private void ApplyDamageWithArmor(int amount, bool isCritical, bool ignoreArmor = false)
    {
        if(!canUseArmor || ignoreArmor)
        {
            currentHealth = (int)Math.Max(0, currentHealth - amount);
            SpawnDamageText(amount, 0, isCritical);
            if (currentHealth <= 0)
            {
                Die();
            }
            return;
        }

        int healthLoss = 0;
        int armorLoss = 0;
        int oldArmor = currentArmor;
        isTakingDamage = true;

        if(forgetTakingDamageCoroutine != null)
        {
            StopCoroutine(forgetTakingDamageCoroutine);
            forgetTakingDamageCoroutine = null;
        }

        if(armorRecoveryCoroutine != null)
        {
            StopCoroutine(armorRecoveryCoroutine);
            armorRecoveryCoroutine = null;
        }

        // deal damage to armor first
        if (currentArmor > 0)
        {
            currentArmor -= amount;
            if (currentArmor < 0)
            {
                amount = Math.Abs(currentArmor);
                currentArmor = 0;
            }
            else
            {
                amount = 0;
            }
        }

        healthLoss = amount;
        armorLoss = oldArmor - currentArmor;

        SpawnDamageText(healthLoss, armorLoss, isCritical);

        // deal damage to health
        currentHealth = (int)Math.Max(0, currentHealth - amount);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDirectEffectDamage(int amount, Effect effect, bool ignoreArmor = false, bool isInvisible = false)
    {
        if (!isInvisible)
        {
            ApplyDamageWithArmor(amount, false, ignoreArmor);
        }

        flashEffect?.Flash();

        if (hurtSound && !audioSource.isPlaying)
        {
            audioSource?.PlayOneShot(hurtSound);
        }

        //var color = Color.white;
        //var text = amount.ToString();
        //float lifeTime = 0.8f;
        //Vector2 initVel = new Vector2(0, 1);
        //float scale = 1;

        //color = EffectConfig.Instance.GetEffectTextColor(effect.EffectType);

        //this.SpawnText(text, color, lifeTime, initVel, scale);

        SetIsHurtTrue();
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

    internal void Freeze(float freezeDuration)
    {
        if (IsFreezing)
        {
            if (freezeDuration > freezeTimeLeft)
            {
                freezeTimeLeft = freezeDuration;
            }

            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Animator animator = GetComponent<Animator>();
        
        if(animator != null)
        {
            animator.speed = 0;
        }

        IsFreezing = true;
        freezeTimeLeft = freezeDuration;

        if(gameObject.active)
        {
            StartCoroutine(Unfreeze());
        }

    }

    private IEnumerator Unfreeze()
    {
        while (freezeTimeLeft > 0)
        {
            freezeTimeLeft -= Time.deltaTime;
            yield return null;
        }

        IsFreezing = false;
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.speed = 1;
        }

        yield break;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public void SafeDelegateOnHurt(Action<DamageData> onHurtHandler)
    {
        OnHurt -= onHurtHandler;
        OnHurt += onHurtHandler;
    }

    public void RemoveDelegateOnHurt(Action<DamageData> onHurtHandler)
    {
        OnHurt -= onHurtHandler;
    }

    public void SafeDelegateOnDeath(Action onDeathHandler)
    {
        OnDeath -= onDeathHandler;
        OnDeath += onDeathHandler;
    }

    public void RemoveDelegateOnDeath(Action onDeathHandler)
    {
        OnDeath -= onDeathHandler;
    }

    public void SafeDelegateOnCanMoveStateChanged(Action<bool> onCanMoveStateChangedHandler)
    {
        OnCanMoveStateChanged -= onCanMoveStateChangedHandler;
        OnCanMoveStateChanged += onCanMoveStateChangedHandler;
    }

    public void RemoveDelegateOnCanMoveStateChanged(Action<bool> onCanMoveStateChangedHandler)
    {
        OnCanMoveStateChanged -= onCanMoveStateChangedHandler;
    }

    public void NotifyCanMoveStateChanged()
    {
        OnCanMoveStateChanged?.Invoke(CanMove());
    }

    // Check if the ray from the character to the position is not blocked by any collider
    public bool CanSeePosition(Vector3 position)
    {
        Vector2 direction = position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
        {
            return true;
        }
        return false;
    }

    public bool ApplyImpulse(Vector2 velocity, float duration = 0.1f)
    {
        if (impulseCoroutine != null)
        {
            return false;
        }
        impulseCoroutine = StartCoroutine(ApplyImpulseCo(velocity, duration));
        return true;
    }

    private IEnumerator ApplyImpulseCo(Vector2 velocity, float duration)
    {
        DisableMovement();
        rb.linearVelocity += velocity*pushScale;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity -= Vector2.zero;
        impulseCoroutine = null;
        EnableMovement();
    }

    internal WeaponControl GetWeaponControl()
    {
        return weaponControl;
    }

    internal float GetSpeed()
    {
        return runSpeed;
    }

    internal void ModifySpeed(float v)
    {
        runSpeed += v;
        OnSpeedChange?.Invoke(runSpeed);
    }

    internal void SafeAddOnSpeedChangeDelegate(Action<float> onSpeedChangeDelegate)
    {
        OnSpeedChange -= onSpeedChangeDelegate;
        OnSpeedChange += onSpeedChangeDelegate;
    }

    internal void RemoveOnSpeedChangeDelegate(Action<float> onSpeedChangeDelegate)
    {
        OnSpeedChange -= onSpeedChangeDelegate;
    }

    public RaceType Race => race;

    public void SetPrimaryWeapon(BaseWeapon weapon)
    {
        primaryWeapon = weapon;
    }

    public void SetSecondaryWeapon(BaseWeapon weapon)
    {
        secondaryWeapon = weapon;
    }


}

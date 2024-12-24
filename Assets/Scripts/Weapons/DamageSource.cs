using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;
    [SerializeField]
    private float criticalChance = 0.0f;
    [SerializeField]
    private float criticalMultiplier = 1.6f;
    [SerializeField]
    private float damage = 0f;
    [SerializeField]
    private bool hasPushEffect = false;
    [SerializeField]
    private float pushScale = 0f;
    [SerializeField]
    private DamageType damageType = DamageType.PHYSIC;
    [SerializeField]
    private Element element;
    [SerializeField]
    private float coolDown = 0f;// 0f means no cooldown by default
    [SerializeField]
    private float coolDownMultiplier = 1f;
    [SerializeField]
    private float accuracy = 0.5f;
    [SerializeField]
    private float dispersion = 10f;

    private float coolDownTimer = 0f;

    public void Awake()
    {
        // save the initial stats
        initialOwner = owner;
        initialDamage = damage;
        initialCriticalChance = criticalChance;
        initialCriticalMultiplier = criticalMultiplier;
        initialCoolDown = coolDown;
        initialHasPushEffect = hasPushEffect;
        initialPushScale = pushScale;
        initialDamageType = damageType;
        initialElement = element;
        initialCoolDownMultiplier = coolDownMultiplier;
    }

    /**
     * Reset the stats of this damage source to the initial values
     * Notes: will also reset the owner of this damage source
     */
    public void ResetStats(bool shouldResetOwner = false)
    {
        owner = shouldResetOwner? initialOwner : null;
        damage = initialDamage;
        criticalChance = initialCriticalChance;
        criticalMultiplier = initialCriticalMultiplier;
        coolDown = initialCoolDown;
        hasPushEffect = initialHasPushEffect;
        damageType = initialDamageType;
        element = initialElement;
        pushScale = initialPushScale;
        coolDownMultiplier = initialCoolDownMultiplier;
    }

    public void Update()
    {
        coolDownTimer = Mathf.Max(0f, coolDownTimer - Time.deltaTime * coolDownMultiplier);
    }

    // Can be used to check if the weapon can attack
    public bool IsCoolDownReset()
    {
        return coolDownTimer <= 0.000001f;
    }

    // Properties
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public float CriticalChance
    {
        get { return criticalChance; }
        set { criticalChance = value; }
    }

    public float CriticalMultiplier
    {
        get { return criticalMultiplier; }
        set { criticalMultiplier = value; }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public bool HasPushEffect
    {
        get { return hasPushEffect; }
        set { hasPushEffect = value; }
    }

    public DamageType DamageType
    {
        get { return damageType; }
        set { damageType = value; }
    }

    public Element Element
    {
        get { return element; }
        set { element = value; }
    }

    private bool IsCriticalHit(float chance)
    {
        return Random.value < chance;
    }

    public void ApplyCoolDown()
    {
        coolDownTimer = coolDown;
    }

    public DamageData GetDamageData(Vector2 sourcePos, Vector2 targetPos, bool ignoreIsElemental = false)
    {
        DamageData damageData = new DamageData();
        damageData.DamageDealer = owner;
        damageData.Damage = damage;
        damageData.IsCritical = IsCriticalHit(criticalChance);
        damageData.SourceElement = element;
        damageData.SourcePosition = sourcePos;
        damageData.TargetPosition = targetPos;
        damageData.PushScale = hasPushEffect? pushScale : 0f;

        // check if the damage is critical and not elemental, then apply the critical multiplier
        if (damageData.IsCritical && (!element.IsElemental || ignoreIsElemental))
        {
            damageData.Damage *= criticalMultiplier;
        }

        return damageData;
    }

    internal DamageData ApplyDamageTo(BaseCharacter target, Vector2 position, bool shouldApplyCoolDown = true)
    {
        if(shouldApplyCoolDown)
        {
            ApplyCoolDown();
        }

        DamageData damageData = GetDamageData(transform.position, target.transform.position);
        damageData.DamageDealer = owner;
        if (damageData.IsCritical && element.IsElemental)
        {
            ApplyElementalEffectToTarget(target);
        }
        target.TakeDamage(damageData);
        return damageData;
    }

    internal void AppyDamageDataTo(DamageData damageData, BaseCharacter target)
    {
        if (damageData.IsCritical && damageData.SourceElement.IsElemental)
        {
            ApplyElementalEffectToTarget(target);
        }
        target.TakeDamage(damageData);
    }

    public bool ApplyElementalEffectToTarget(BaseCharacter target)
    {
        if (element == null || !element.IsElemental || target == null)
        {
            return false;
        }

        // apply elemental effect
        Effect effectPrefab = element.Effect;
        if (effectPrefab != null)
        {
            Effect effectInstance = null;
            if(effectPrefab.AttachToTarget)
            {
                effectInstance = Instantiate(effectPrefab, target.transform.position, Quaternion.identity, target.transform);
            }
            else
            {
                effectInstance = Instantiate(effectPrefab, target.transform.position, Quaternion.identity);
            }
            effectInstance.StartEffect(target);
        }

        return true;
    }

    // used to reset the stats of this damage source when needed
    private GameObject initialOwner;
    private float initialDamage;
    private float initialCriticalChance;
    private float initialCriticalMultiplier;
    private float initialCoolDown;
    private float initialPushScale;
    private DamageType initialDamageType;
    private Element initialElement;
    private bool initialHasPushEffect;
    private float initialCoolDownMultiplier;
    private float initialAccuracy;
    private float initialDispersion;

    //get set methods
    public float PushScale
    {
        get { return pushScale; }
        set { pushScale = value; }
    }

    public float CoolDown
    {
        get { return coolDown; }
        set { coolDown = value; }
    }

    public float CoolDownTimer
    {
        get { return coolDownTimer; }
    }

    public float CoolDownMultiplier
    {
        get { return coolDownMultiplier; }
        set { coolDownMultiplier = value; }
    }

    public void SetCoolDownTimer(float value)
    {
        coolDownTimer = value;
    }

    public void SetCoolDownMultiplier(float value)
    {
        coolDownMultiplier = value;
    }

    public void SetCriticalChance(float value)
    {
        criticalChance = value;
    }

    public void SetCriticalMultiplier(float value)
    {
        criticalMultiplier = value;
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    public void SetHasPushEffect(bool value)
    {
        hasPushEffect = value;
    }

    public void SetDamageType(DamageType value)
    {
        damageType = value;
    }

    public void SetElement(Element value)
    {
        element = value;
    }

    public void SetPushScale(float value)
    {
        pushScale = value;
    }

    public void SetOwner(GameObject value)
    {
        owner = value;
    }

    public void SetAccuracy(float value)
    {
        accuracy = value;
    }

    public void SetDispersion(float value)
    {
        dispersion = value;
    }

    public Vector2 GetDispersedLookDir(Vector2 dir)
    {
        float angle = Random.Range(-dispersion, dispersion) * (1-accuracy);
        return Quaternion.Euler(0, 0, angle) * dir.normalized;
    }
}


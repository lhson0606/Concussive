using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;
    [SerializeField]
    private float criticalChance = 0.0f;
    [SerializeField]
    private float criticalMultiplier;
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

    public DamageData GetDamageData(Vector2 sourcePos, Vector2 targetPos)
    {
        DamageData damageData = new DamageData();
        damageData.DamageDealer = owner;
        damageData.Damage = damage;
        damageData.IsCritical = IsCriticalHit(criticalChance);
        damageData.SourceElement = element;
        damageData.SourcePosition = sourcePos;
        damageData.TargetPosition = targetPos;
        damageData.PushScale = hasPushEffect? pushScale : 0f;
        return damageData;
    }

    internal void ApplyDamageTo(BaseCharacter target, Vector2 position, bool shouldApplyCoolDown = true)
    {
        if(shouldApplyCoolDown)
        {
            ApplyCoolDown();
        }

        DamageData damageData = GetDamageData(transform.position, target.transform.position);
        if(damageData.IsCritical)
        {
            if(!ApplyElementalEffectToTarget(target))
            {
                damageData.Damage *= criticalMultiplier;
            }
        }
        target.TakeDamage(damageData);
    }

    private bool ApplyElementalEffectToTarget(BaseCharacter target)
    {
        if (element == null || !element.IsElemental)
        {
            return false;
        }

        // apply elemental effect
        Effect effectPrefab = element.Effect;
        if (effectPrefab != null)
        {
            Effect effectInstance = Instantiate(effectPrefab, target.transform.position, Quaternion.identity, target.transform);
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
}


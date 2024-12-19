using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;
    [SerializeField]
    private float criticalChance = 0.05f;
    [SerializeField]
    private float criticalMultiplier;
    [SerializeField]
    private float damage = 0f;
    [SerializeField]
    private bool havePushEffect = false;
    [SerializeField]
    private DamageType damageType = DamageType.PHYSIC;
    [SerializeField]
    private Element element = null;

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

    public bool HavePushEffect
    {
        get { return havePushEffect; }
        set { havePushEffect = value; }
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool IsCriticalHit(float chance)
    {
        return Random.value < chance;
    }

    public void ApplyDamageTo(BaseCharacter target)
    {
        DamageData damageData = new DamageData();
        damageData.Damage = damage;
        damageData.IsCritical = IsCriticalHit(criticalChance);
        damageData.SourceElement = element;
        damageData.SourcePosition = (owner != null)? owner.transform.position : transform.position;
        damageData.TargetPosition = target.transform.position;
        damageData.HavePushEffect = havePushEffect;

        // Calculate crit damage
        if (damageData.IsCritical)
        {
            if (element != null && element.IsElemental)
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
                damageData.Damage *= criticalMultiplier;
            }
        }

        target.TakeDamage(damageData);
    }
}


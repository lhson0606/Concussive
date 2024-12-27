using UnityEngine;

public class KnightShieldSkill : BaseSkill
{
    [SerializeField]
    private GameObject shieldSpellObjPrefab;
    [SerializeField]
    private float shieldDuration = 5f;

    public override void OnUse()
    {
        base.OnUse();

        if(!owner)
        {
            return;
        }

        //spawn shield
        GameObject shieldSpellObj = Instantiate(shieldSpellObjPrefab, owner.transform.position, Quaternion.identity, owner.transform);
        SpellShieldObj shield = shieldSpellObj.GetComponent<SpellShieldObj>();
        shield.SetOwner(owner);
        Destroy(shield.gameObject, shieldDuration);
    }
}

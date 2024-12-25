using UnityEngine;

public class DoubleTripleSecondary : BaseWeapon
{
    [SerializeField]
    private GameObject projectilePrefab;

    public override void DoAttack()
    {
        base.DoAttack();

        if(!damageSource.IsCoolDownReset())
        {
            return;
        }
    }
}

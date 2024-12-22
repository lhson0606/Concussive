using UnityEngine;

public class StaffScript : BaseWeapon
{
    [SerializeField]
    private GameObject staffTip;
    [SerializeField]
    private GameObject staffProjectilePrefab;

    public override void DoAttack()
    {
        base.DoAttack();
        if(!damageSource.IsCoolDownReset())
        {
            return;
        }

        animator?.SetTrigger("Attack");
        damageSource.ApplyCoolDown();
    }

    // Call from animation
    public void SpawnProjectile()
    {
        GameObject projectile = Instantiate(staffProjectilePrefab, staffTip.transform.position, Quaternion.identity);
        StaffProjectile staffProjectile = projectile.GetComponent<StaffProjectile>();
        staffProjectile.SetDamageSource(damageSource);
        staffProjectile.SetDirection(damageSource.GetDispersedLookDir(owner.LookDir));
        staffProjectile.SetParentWeapon(this);
        staffProjectile.Launch();
    }
}

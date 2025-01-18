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

    public override void SetAsMainWeapon(BaseCharacter owner)
    {
        base.SetAsMainWeapon(owner);
        base.ShouldAlterRenderOrder = false;
        // set render order to character +1
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
    }

    // Call from animation
    public void SpawnProjectile()
    {
        GameObject projectile = Instantiate(staffProjectilePrefab, staffTip.transform.position, Quaternion.identity);
        StaffProjectile staffProjectile = projectile.GetComponent<StaffProjectile>();
        staffProjectile.SetAllNecessities(owner.LookDir, this);
        staffProjectile.Launch();
    }
}

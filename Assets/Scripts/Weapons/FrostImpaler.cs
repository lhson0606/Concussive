using UnityEngine;

public class FrostImpaler : BaseWeapon
{
    [SerializeField]
    private GameObject iceImpalerProjPrefab;

    public override void DoAttack()
    {
        base.DoAttack();
        if (!damageSource.IsCoolDownReset())
        {
            return;
        }

        animator?.SetTrigger("Fire");
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
        // convert right direction to quaternion
        Vector3 direction = owner.LookDir;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject projectile = Instantiate(iceImpalerProjPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        IceImpalerProj projectileScript = projectile.GetComponent<IceImpalerProj>();
        projectileScript.SetAllNecessities(projectile.transform.right, this);
        projectileScript.Launch();
    }
}

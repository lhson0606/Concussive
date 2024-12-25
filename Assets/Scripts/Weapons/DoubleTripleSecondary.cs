using UnityEngine;
using System.Collections;

public class DoubleTripleSecondary : BaseWeapon
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float angleBetweenProjectiles = 15f;
    [SerializeField] private float timeBetweenWaves = 0.1f;

    public override void DoAttack()
    {
        base.DoAttack();

        if (!damageSource.IsCoolDownReset() || !projectilePrefab)
        {
            return;
        }

        StartCoroutine(LaunchProjectiles());
        damageSource.ApplyCoolDown();
    }

    private IEnumerator LaunchProjectiles()
    {
        // Launch first wave
        for (int i = 0; i < 5; i++)
        {
            // Calculate projectile direction
            Quaternion rotation = Quaternion.Euler(0f, 0f, -angleBetweenProjectiles * 2 + i * angleBetweenProjectiles);
            Vector3 direction = rotation * owner.LookDir;

            // Instantiate and launch projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
            BaseProjectile projectileScript = projectile.GetComponent<BaseProjectile>();
            projectileScript.SetAllNecessities(damageSource, direction, this);
            projectileScript.Launch();

            // Slight delay between each projectile in the first wave (optional)
            yield return new WaitForSeconds(0.05f);
        }

        // Wait for specified time between waves
        yield return new WaitForSeconds(timeBetweenWaves);

        // Launch second wave
        for (int i = 0; i < 5; i++)
        {
            // Calculate projectile direction
            Quaternion rotation = Quaternion.Euler(0f, 0f, -angleBetweenProjectiles * 2 + i * angleBetweenProjectiles);
            Vector3 direction = rotation * owner.LookDir;

            // Instantiate and launch projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
            BaseProjectile projectileScript = projectile.GetComponent<BaseProjectile>();
            projectileScript.SetAllNecessities(damageSource, direction, this);
            projectileScript.Launch();

            // Slight delay between each projectile in the second wave (optional)
            yield return new WaitForSeconds(0.05f);
        }
    }
}
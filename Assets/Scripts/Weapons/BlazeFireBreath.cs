using System;
using System.Collections;
using UnityEngine;

public class BlazeFireBreath : BaseWeapon
{
    [SerializeField]
    private GameObject blazeFireBallProjectilePrefab;
    [SerializeField]
    private float attackRate = 0.5f;
    [SerializeField]
    private int numberOfFireBalls = 3;

    private Coroutine attackCoroutine;

    public override void DoAttack()
    {
        base.DoAttack();

        if(!damageSource.IsCoolDownReset())
        {
            return;
        }

        damageSource.ApplyCoolDown();

        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(LaunchFireBall());
        }
    }

    private IEnumerator LaunchFireBall()
    {
        for (int i = 0; i < numberOfFireBalls; i++)
        {
            LaunchSingleFireBall();
            yield return new WaitForSeconds(attackRate);
        }

        attackCoroutine = null;
    }

    private void LaunchSingleFireBall()
    {
        Vector3 direction = owner.LookDir;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        BlazeFireBall blazeFireBall = Instantiate(blazeFireBallProjectilePrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<BlazeFireBall>();
        blazeFireBall.SetAllNecessities(owner.LookDir, this, true);
        blazeFireBall.Launch();
    }
}

using System.Collections;
using UnityEngine;

public class ArcherEnemy : RangedEnemy
{
    private BowScript bowScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        bowScript = primaryWeapon.GetComponent<BowScript>();

        if (bowScript == null)
        {
            Debug.LogError("BowScript not found");
        }
    }

    private bool isCharging = false;
    private bool isWaitingForNextAttack = false;
    // Update is called once per frame
    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        //if(!GetPrimaryWeapon().GetDamageSource().IsCoolDownReset() && target != null)
        //{
        //    LookAtPosition = target.transform.position;
        //}
        LookAtPosition = target.transform.position;
        if (!isCharging && !isWaitingForNextAttack)
        {
            bowScript.DoAttack();
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        isCharging = true;
        // wait for the bow to be fully drawn .. randomly between 1.5 and 2.5 seconds
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        bowScript.ReleaseAttack();
        isCharging = false;
        StartCoroutine(WaitForNextAttack(Random.Range(1f, 2f)));
    }

    private IEnumerator WaitForNextAttack(float duration)
    {
        isWaitingForNextAttack = true;
        yield return new WaitForSeconds(duration);
        isWaitingForNextAttack = false;
    }

    public override void Update()
    {
        base.Update();
    }
}
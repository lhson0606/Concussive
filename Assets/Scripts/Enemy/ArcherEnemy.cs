using System;
using System.Collections;
using UnityEngine;

public class ArcherEnemy : RangedEnemy
{
    private BowScript bowScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        BowScript bowScript = base.primaryWeapon.GetComponent<BowScript>();

        if (bowScript == null)
        {
            Debug.LogError("BowScript not found");
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();        
        if (bowScript == null)
        {
            bowScript = base.primaryWeapon.GetComponent<BowScript>();
            return;
        }

        bowScript.DoAttack();
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        // wait for the bow to be fully drawn .. about 1.5 seconds
        yield return new WaitForSeconds(1.5f);
        bowScript.ReleaseAttack();
    }
}

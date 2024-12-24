using System;
using System.Collections;
using UnityEngine;

public class OgreBoss : Enemy
{
    [SerializeField]
    private float meleeAttackRange = 2f;
    [SerializeField]
    private float rangedAttackRange = 8f;
    [SerializeField]
    private float spearModeThreshold = 6f;

    private SpearScript spear;

    public override void Start()
    {
        base.Start();
        SafeAddActivationDelegate(DoBossIntro);
        spear = GetPrimaryWeapon() as SpearScript;
    }

    public override void Update()
    {
        base.Update();
        if (HasTarget() && IsTargetInSight())
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance > spearModeThreshold && spear.CurrentMode == HybridWeapon.HybridMode.Melee)
            {
                attackRadius = rangedAttackRange;
                spear.OnSpecialModeTriggered();
            } else if (distance <= spearModeThreshold && spear.CurrentMode == HybridWeapon.HybridMode.Ranged)
            {
                attackRadius = meleeAttackRange;
                spear.OnSpecialModeTriggered();
            }
        }
    }

    public override void AttackCurrentTarget()
    {
        LookAtPosition = target.transform.position;
        // damageSource?.ApplyCoolDown();
        spear.DoAttack();

        if(spear.CurrentMode == HybridWeapon.HybridMode.Ranged)
        {
            rb.linearVelocity = (target.transform.position - transform.position).normalized * runSpeed;
        }
    }

    private IEnumerator ReleaseCharge(float duration)
    {
        yield return new WaitForSeconds(duration);
        spear.ReleaseAttack();
    }

    public void DoBossIntro()
    {
        // Show the boss intro screen
        FlashScreenController flashScreenController = FindAnyObjectByType<FlashScreenController>();
        if (flashScreenController != null)
        {
            GameObject bossSkillPrefab = Resources.Load<GameObject>("Prefabs/UI/Boss/OgreIntro");

            flashScreenController.ShowFlashScreen(bossSkillPrefab);
        }
    }
}

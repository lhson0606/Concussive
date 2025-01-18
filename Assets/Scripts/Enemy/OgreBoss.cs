using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OgreBoss : Enemy
{
    [SerializeField]
    private float meleeAttackRange = 2f;
    [SerializeField]
    private float rangedAttackRange = 8f;
    [SerializeField]
    private float spearModeThreshold = 6f;

    private SpearScript spear;
    private SecondariesModule secondariesModule;
    private SkillModule skillModule;
    public GameObject bossUI; // Reference to the BossUI GameObject
    public Image healthBarFill; // Reference to the HealthBarFill Image

    public override void Start()
    {
        base.Start();
        SafeAddActivationDelegate(DoBossIntro);
        spear = GetPrimaryWeapon() as SpearScript;
        secondariesModule = GetComponentInChildren<SecondariesModule>();
        skillModule = GetComponentInChildren<SkillModule>();
        secondariesModule.SetOwner(this);
    }

    public override void Update()
    {
        UpdateHealthBar();
        base.Update();
        if (HasTarget() && IsTargetInSight() && isActivated)
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance > spearModeThreshold && spear.CurrentMode == HybridWeapon.HybridMode.Melee)
            {
                spear.OnSpecialModeTriggered();
            }
            else if (distance <= spearModeThreshold && spear.CurrentMode == HybridWeapon.HybridMode.Ranged)
            {
                attackRadius = meleeAttackRange;
                spear.OnSpecialModeTriggered();
            }
            secondariesModule.AimAndFireWithProbability(0.6f);

            if (spear.CurrentMode == HybridWeapon.HybridMode.Ranged)
            {
                if (damageSource.IsCoolDownReset())
                {
                    AttackCurrentTarget();
                }
            }

            // use skill with probability
            bool hasUsed = skillModule.UseRandomSkillWithProbability(0.4f);

            if(hasUsed)
            {
                float distanceToPlayerCamera = Vector3.Distance(transform.position, player.transform.position);
                playerController.ShakePlayerCamera(distanceToPlayerCamera);
                audioSource?.PlayOneShot(angryNoise);
            }
        }
    }

    public override void AttackCurrentTarget()
    {
        LookAtPosition = target.transform.position;
        // damageSource?.ApplyCoolDown();
        spear.DoAttack();
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

        // Enable the BossUI
        if (bossUI != null)
        {
            bossUI.SetActive(true);
        }
        // Update the health bar fill based on the boss's current health
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercentage = GetHealthPercentage();
            healthBarFill.fillAmount = healthPercentage;
        }
    }

    private float GetHealthPercentage()
    {
        if (currentHealth != null)
        {
            return (float)currentHealth / (float)maxHealth;
        }
        return 0f;
    }
}

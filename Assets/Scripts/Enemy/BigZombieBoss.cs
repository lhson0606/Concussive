using UnityEngine;
using UnityEngine.UI;

public class BigZombieBoss : Enemy
{
    private SkillModule skillModule;
    private SecondariesModule secondariesModule;
    public GameObject bossUI; // Reference to the BossUI GameObject
    public Image healthBarFill; // Reference to the HealthBarFill Image

    public override void Start()
    {
        base.Start();
        SafeAddActivationDelegate(DoBossIntro);
        secondariesModule = GetComponentInChildren<SecondariesModule>();
        skillModule = GetComponent<SkillModule>();
        secondariesModule.SetOwner(this);
    }

    public override void Update()
    {
        base.Update();
        UpdateHealthBar();
        if (HasTarget() && IsTargetInSight() && isActivated)
        {
            // use skill with probability
            bool hasUsed = skillModule.UseRandomSkillWithProbability(0.3f);
            secondariesModule.AimAndFireWithProbability(0.4f);

            if (hasUsed)
            {
                float distanceToPlayerCamera = Vector3.Distance(transform.position, player.transform.position);
                playerController.ShakePlayerCamera(distanceToPlayerCamera);
                audioSource?.PlayOneShot(angryNoise);
            }
        }
    }

    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        GetPrimaryWeapon()?.DoAttack();
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
        if (maxHealth <= 0)
        {
            return 0;
        }

        return (float)currentHealth / (float)maxHealth;
    }

    public void DoBossIntro()
    {
        // Enable the BossUI
        if (bossUI != null)
        {
            bossUI.SetActive(true);
        }
    }
}

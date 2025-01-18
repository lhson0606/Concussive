using UnityEngine;
using UnityEngine.UI;

public class BlazeScript : RangedEnemy
{
    private SecondariesModule secondariesModule;
    public GameObject bossUI; // Reference to the BossUI GameObject
    public Image healthBarFill; // Reference to the HealthBarFill Image


    public override void Start()
    {
        base.Start();
        SafeAddActivationDelegate(DoBossIntro);
        secondariesModule = GetComponentInChildren<SecondariesModule>();
        secondariesModule.SetOwner(this);
    }

    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        primaryWeapon.DoAttack();
    }

    public override void OnEnemyUpdate()
    {
        base.OnEnemyUpdate();
        UpdateHealthBar();
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

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercentage = GetHealthPercentage();
            healthBarFill.fillAmount = healthPercentage;
        }
    }
}

using UnityEngine;

public class BigZombieBoss : Enemy
{
    private SkillModule skillModule;
    private SecondariesModule secondariesModule;

    public override void Start()
    {
        base.Start();
        secondariesModule = GetComponentInChildren<SecondariesModule>();
        skillModule = GetComponent<SkillModule>();
        secondariesModule.SetOwner(this);
    }

    public override void Update()
    {
        base.Update();
        if (HasTarget() && IsTargetInSight() && isActivated)
        {
            // use skill with probability
            bool hasUsed = skillModule.UseRandomSkillWithProbability(0.3f);
            secondariesModule.AimAndFireWithProbability(0.8f);

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
}

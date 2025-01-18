using UnityEngine;

public class IceBornDemon : RangedEnemy
{
    private Animator bodyAnimator;
    private IceBornDemonBody iceBornDemonBody;

    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        bodyAnimator = transform.Find("Body")?.GetComponent<Animator>();
        iceBornDemonBody = GetComponentInChildren<IceBornDemonBody>();
        iceBornDemonBody.OnLaunchIceImpaler += LaunchIceImpaler;
    }

    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();

        if(!isAttacking && damageSource.IsCoolDownReset() && HasTarget())
        {
            isAttacking = true;
            bodyAnimator?.SetTrigger("Attack");
        }
    }

    // Call from animation
    public void LaunchIceImpaler()
    {
        isAttacking = false;
        GetPrimaryWeapon().DoAttack();
    }
}

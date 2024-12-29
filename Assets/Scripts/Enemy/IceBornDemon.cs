using UnityEngine;

public class IceBornDemon : RangedEnemy
{
    private Animator bodyAnimator;
    private IceBornDemonBody iceBornDemonBody;

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
        bodyAnimator?.SetTrigger("Attack");
    }

    // Call from animation
    public void LaunchIceImpaler()
    {
        GetPrimaryWeapon().DoAttack();
    }
}

using UnityEngine;

public class ShamanEnemyScript : RangedEnemy
{
    private SecondariesModule secondariesModule;

    protected override void Awake()
    {
        base.Awake();
        secondariesModule = GetComponentInChildren<SecondariesModule>();
    }

    public override void Start()
    {
        base.Start();
        secondariesModule.SetOwner(this);
    }

    public override void OnEnemyUpdate()
    {
        base.OnEnemyUpdate();
        if(canSeeTarget && isTargetInAttackRange)
        {
            secondariesModule.AimAndFireWithProbability(0.4f);
        }
    }

    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        GetPrimaryWeapon()?.DoAttack();
    }
}

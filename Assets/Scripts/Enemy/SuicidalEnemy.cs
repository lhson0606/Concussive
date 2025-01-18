using UnityEngine;

public class SuicidalEnemy : Enemy
{
    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        GetPrimaryWeapon()?.DoAttack();
    }
}

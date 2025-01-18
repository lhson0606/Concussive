using UnityEngine;

public class SwordEnemy : Enemy
{
    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        GetPrimaryWeapon()?.DoAttack();
    }
}

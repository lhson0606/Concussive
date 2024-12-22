using UnityEngine;

public class ShamanEnemyScript : RangedEnemy
{
    public override void AttackCurrentTarget()
    {
        base.AttackCurrentTarget();
        GetPrimaryWeapon()?.DoAttack();
    }
}

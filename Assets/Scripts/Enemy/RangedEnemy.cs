using UnityEngine;

public class RangedEnemy : Enemy
{
    public override void AttackCurrentTarget() 
    { 
        base.AttackCurrentTarget();

        if (primaryWeapon.GetDamageSource().IsCoolDownReset() && target != null)
        {
            LookAtPosition = target.transform.position;
        }
    }
}

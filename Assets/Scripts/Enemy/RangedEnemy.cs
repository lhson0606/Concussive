using UnityEngine;

public class RangedEnemy : Enemy
{
    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (target != null)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > attackRadius)
            {
                target = null;
            }
            else
            {
                if(primaryWeapon.GetDamageSource().IsCoolDownReset() && target != null)
                {
                    LookAtPosition = target.transform.position;
                }                
            }
        }
    }

    public float AttackRange
    {
        get
        {
            return attackRadius;
        }
    }
}

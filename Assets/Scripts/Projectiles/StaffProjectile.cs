using System;
using UnityEngine;

public class StaffProjectile : BaseProjectile
{
    public override void OnHit(Collider2D collision)
    {
        base.OnHit(collision);
        DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource, false);
        AudioUtils.PlayAudioClipAtPoint(parentWeapon.GetOnHitSound(), transform.position);
    }
}

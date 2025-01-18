using UnityEngine;

public class SpearProjectile : BaseProjectile
{
    private SpearScript spear;

    public override void OnHit(Collider2D collision)
    {
        base.OnHit(collision);
        spear = parentWeapon as SpearScript;
        int damageRes = DamageUtils.TryToApplyDamageToWithResult(owner, collision, damageSource, false);

        if (audioSource && !audioSource.isPlaying)
        {
            if (damageRes > 0 && spear.GetRangedHitSound())
            {
                audioSource.PlayOneShot(spear.GetRangedHitSound());
            }
            else if (spear.GetRangedHitObstacleSound())
            {
                audioSource.PlayOneShot(spear.GetRangedHitObstacleSound());
            }
        }
    }
}

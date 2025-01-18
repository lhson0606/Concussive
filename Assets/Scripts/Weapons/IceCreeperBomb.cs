using System.Collections.Generic;
using UnityEngine;

public class IceCreeperBomb : BaseWeapon
{
    [SerializeField]
    private AudioClip explosionSound;

    private Collider2D explosionCol;

    protected override void Awake()
    {
        base.Awake();
        explosionCol = transform.Find("Explosion").gameObject.GetComponent<Collider2D>();   
    }

    public override void DoAttack()
    {
        base.DoAttack();
        animator?.SetTrigger("Attack");
    }

    public void Explode()
    {
        AudioUtils.PlayAudioClipAtPoint(explosionSound, transform.position);

        var hitColliders = new List<Collider2D>(10);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;

        var count = Physics2D.OverlapCollider(explosionCol, contactFilter, hitColliders);

        for (int i = 0; i < count; i++)
        {
            DamageUtils.TryToApplyDamageTo(owner?.gameObject, hitColliders[i], damageSource);
        }

        //#bug: some how we cannot detect collider of the owner, so I just directly call Die() here
        owner.Die();
    }
}

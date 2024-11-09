using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
    [SerializeField]
    private Collider2D attackCollider;

    public void TriggerAttack()
    {
        var hitColliders = new Collider2D[10];
        var count = Physics2D.OverlapCollider(attackCollider, new ContactFilter2D(), hitColliders);
        Debug.Log($"Hit {count} colliders");

        for (int i = 0; i < count; i++)
        {
            IDamageable damageable = hitColliders[i].GetComponent<IDamageable>();
            BaseCharacter target = hitColliders[i].GetComponent<BaseCharacter>();

            if (damageable != null)
            {
                DamageData damageData = base.CalculateDamage(owner, target);
                damageable.TakeDamage(damageData);
                base.OnHit();
            }
        }
    }
}

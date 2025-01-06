using UnityEngine;

public class BlazeFireBall : BaseProjectile
{
    [SerializeField]
    private GameObject groundFirePrefab; // Reference to the ground fire effect prefab

    public override void OnHit(Collider2D collision)
    {
        int applyDamageResult = DamageUtils.TryToApplyDamageToWithResult(owner, collision, damageSource, false);
        if (applyDamageResult <= 0)
        {
            if (Random.value < damageSource.CriticalChance)
            {
                if (hitSound != null)
                {
                    AudioUtils.PlayAudioClipAtPoint(hitSound, transform.position);
                }

                // Create a fire on the ground here
                CreateFireOnImpact(transform.position);
            }
        }
        else
        {
            if (hitSound != null)
            {
                AudioUtils.PlayAudioClipAtPoint(hitSound, transform.position);
            }
        }

        animator?.SetTrigger("Impact");
    }

    private void CreateFireOnImpact(Vector2 position)
    {
        if (groundFirePrefab != null)
        {
            FireBorn fireBorn = Instantiate(groundFirePrefab, position, Quaternion.identity).GetComponent<FireBorn>();
            fireBorn.SetOwner(owner);
        }
        else
        {
            Debug.LogWarning("Ground fire prefab is not assigned.");
        }
    }
}
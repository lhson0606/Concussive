using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent (typeof(AudioSource))]
public class UnbreakableVase : MonoBehaviour, IDamageable
{
    [SerializeField]
    AudioClip impactSound;

    private AudioSource audioSource;

    public void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        if (impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    public void TakeDirectEffectDamage(int amount, Effect effect, bool ignoreArmor = false, bool isInvisible = false)
    {
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}

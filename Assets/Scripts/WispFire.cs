using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(AudioSource))]
public class WispFire : MonoBehaviour
{
    [SerializeField]
    private AudioClip hitSound;

    private AudioSource audioSource;
    private DamageSource damageSource;

    internal void SetOwner(BaseCharacter owner)
    {
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }

        damageSource.Owner = owner.gameObject;
    }

    private void Awake()
    {
        damageSource = GetComponent<DamageSource>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!damageSource.IsCoolDownReset() || damageSource.Owner?.tag == collision.tag)
        {
            return;
        }

        bool hit = DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource);

        if (hit && audioSource && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}

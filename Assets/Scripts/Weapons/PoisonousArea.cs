using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(AudioSource))]
public class PoisonousArea : MonoBehaviour
{
    [SerializeField]
    private float timeToLive = 5f;
    [SerializeField]
    private AudioClip poisonousSoundEffect;

    private DamageSource damageSource;
    private AudioSource audioSource;
    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageSource = GetComponent<DamageSource>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();


        if (audioSource != null)
        {
            audioSource.clip = poisonousSoundEffect;
            audioSource.Play();
            audioSource.loop = true;
        }

        Destroy(gameObject, timeToLive);
    }

    // Update is called once per frame
    void Update()
    {
        if (damageSource.IsCoolDownReset())
        {
            DealDamage();
        }
    }

    private void DealDamage()
    {
        List<Collider2D> victims = GetVictims();

        foreach (Collider2D victim in victims)
        {
            BaseCharacter victimCharacter = victim.GetComponent<BaseCharacter>();
            if(victimCharacter != null)
            {
                DamageData damageData = damageSource.GetDamageData(transform.position, victim.transform.position);
                if (victimCharacter.Race == RaceType.UNDEAD ||  victimCharacter.Race == RaceType.ORC)
                {
                    damageData.Damage = 0;
                }
                DamageUtils.TryToApplyDamageDataTo(gameObject, victim, damageData, damageSource);
            }            
        }

        damageSource.ApplyCoolDown();
    }

    private List<Collider2D> GetVictims()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<Collider2D> colliders = new List<Collider2D>();
        col.Overlap(contactFilter, colliders);
        return colliders;
    }
}

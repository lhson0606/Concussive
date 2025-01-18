using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(DamageSource))]
public class FireBorn : MonoBehaviour
{
    [SerializeField]
    private float timeToLive = 8.0f;
    [SerializeField]
    private AudioClip fireSound;

    private DamageSource damageSource; 
    private Collider2D col;
    private AudioSource audioSource; 

    private GameObject owner;

    void Awake()
    {
        damageSource = GetComponent<DamageSource>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
        damageSource.SetOwner(owner);
    }

    void Start()
    {
        if(audioSource != null && fireSound!=null)
        {
            audioSource.clip = fireSound;
            audioSource.Play();
            audioSource.loop = true;
        }

        Destroy(gameObject, timeToLive);
    }

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
            if(ReferenceEquals(victim, owner))
            {
                continue;
            }

            BaseCharacter victimCharacter = victim.GetComponent<BaseCharacter>();
            if (victimCharacter != null && victimCharacter.Race != RaceType.DEMON)
            {
                DamageData damageData = damageSource.GetDamageData(transform.position, victim.transform.position);
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

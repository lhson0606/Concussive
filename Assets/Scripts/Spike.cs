using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(Collider2D))]
public class Spike : MonoBehaviour
{
    private DamageSource damageSource;
    private Collider2D col;

    private bool isUp = false;

    private void Awake()
    {
        damageSource = this.GetComponent<DamageSource>();
        col = this.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isUp)
        {
            return;
        }

        if(damageSource.IsCoolDownReset())
        {
            DealDamage();
        }
    }

    public void SpikeUp()
    {
        isUp = true;
    }

    public void SpikeDown()
    {
        isUp = false;
    }

    private void DealDamage()
    {
        List<Collider2D> victims = GetVictims();

        foreach(Collider2D victim in victims)
        {
            DamageUtils.TryToApplyDamageTo(gameObject, victim, damageSource);
        }
    }

    private List<Collider2D> GetVictims()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<Collider2D> colliders = new List<Collider2D>();
        col.Overlap(contactFilter, colliders);
        return colliders;
    }
}


using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ArrowScript : BaseProjectile
{
    [SerializeField]
    private AudioClip arrowHitSound;

    private SimpleFlashEffect flashEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    public void OnProjectileFullyCharged()
    {
        flashEffect = GetComponent<SimpleFlashEffect>();
        flashEffect?.Flash();
    }

    public override void OnHit(Collider2D collision)
    {
        base.OnHit(collision);
        audioSource?.PlayOneShot(arrowHitSound);
        DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource, false);
    }
}

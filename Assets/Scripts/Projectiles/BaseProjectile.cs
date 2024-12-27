using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseProjectile : SlowMotionObject, IDamageable
{
    protected Rigidbody2D rb;
    [SerializeField]
    protected float speed = 10f;
    [SerializeField]
    protected float lifeTime = 5f;
    [SerializeField]
    protected float delayDestroyTime = 0.5f;
    [SerializeField]
    protected bool stickToTarget = false;
    [SerializeField]
    private List<string> collideTags = new() { "Obstacle", "Door", "Enemy", "Player", "Wall"};

    protected DamageSource damageSource;
    protected Vector2 direction;
    protected Collider2D col;
    protected BaseWeapon parentWeapon;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    protected TrailRenderer trailRenderer;

    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();

        if (isInitialized)
        {
            return;
        }

        Init();
    }

    private void Init()
    {
        isInitialized = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer)
        {
            trailRenderer.emitting = false;
        }
        col.enabled = false;
    }

    public void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        if(GetOwner()?.tag == damageData.DamageDealer?.tag)
        {
            return;
        }
        this.gameObject.tag = damageData.DamageDealer ? damageData.DamageDealer.tag : "Untagged";
        // reverse the arrow direction
        rb.linearVelocity *= -1;
        // reverse the arrow direction
        transform.Rotate(0, 0, 180);
    }

    public void TakeDirectEffectDamage(int amount, Effect effect, bool isInvisible = false)
    {
    }

    internal void Launch()
    {
        if(!isInitialized)
        {
            Init();
        }

        transform.parent = null;

        rb.linearVelocity = damageSource.GetDispersedLookDir(direction).normalized * speed;
        col.enabled = true;
        StartCoroutine(DestroyAfter(lifeTime + delayDestroyTime));
        if (trailRenderer)
        {
            trailRenderer.emitting = true;
        }
        OnLaunch();
    }

    private IEnumerator DestroyAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    internal void SetDamageSource(DamageSource damageSource)
    {
        this.damageSource = damageSource;
    }

    internal void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    internal void SetParentWeapon(BaseWeapon parentWeapon)
    {
        this.parentWeapon = parentWeapon;
    }

    public GameObject GetOwner()
    {
        // unity overloads to check if the game object is destroyed
        if (parentWeapon?.GetOwner()?.gameObject!= null)
        {
            return parentWeapon.GetOwner().gameObject;
        }
        return null;
    }

    public bool IsOwner(GameObject gameObject)
    {
        return ReferenceEquals(GetOwner(), gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.isTrigger)
        //{
        //    return;
        //}

        if(!collideTags.Contains(collision.gameObject.tag))
        {
            return;
        }

        // Ensure the collision is not with the damage source owner or the parent weapon
        if (damageSource == null ||
            damageSource.Owner?.tag == collision.gameObject.tag ||
            ReferenceEquals(collision.gameObject, parentWeapon.gameObject) ||
            ReferenceEquals(collision.gameObject, damageSource.Owner))
        {
            return;
        }

        if (trailRenderer)
        {
            trailRenderer.emitting = false;
        }

        OnHit(collision);

        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (stickToTarget)
        {
            transform.SetParent(collision.transform);
        }
        else
        {
            spriteRenderer.enabled = false;
        }

        if(gameObject.active)
        {
            StartCoroutine(DestroyAfter(delayDestroyTime));
        }        
    }

    public virtual void OnLaunch()
    {
        // Implement specific hit logic in derived classes
    }

    public virtual void OnHit(Collider2D collision)
    {
        // Implement specific hit logic in derived classes
    }

    public void SetAllNecessities(DamageSource damageSource, Vector2 direction, BaseWeapon parentWeapon)
    {
        SetDamageSource(damageSource);
        SetDirection(direction);
        SetParentWeapon(parentWeapon);
    }
}

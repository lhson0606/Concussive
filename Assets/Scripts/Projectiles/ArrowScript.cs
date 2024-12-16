using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ArrowScript : SlowMotionObject
{
    [SerializeField]
    private float speed = 16f;
    [SerializeField]
    private float timeToLive = 5f;
    [SerializeField]
    private AudioClip arrowHitSound;

    private AudioSource audioSource;

    private SimpleFlashEffect flashEffect;
    private Rigidbody2D rb;

    private List<string> collideTags = new() { "Obstacle", "Door", "Enemy"};
    private BaseCharacter owner;
    private BaseWeapon parentWeapon;
    private TrailRenderer trailRenderer;

    private bool isFlying = false;

    public void OnProjectileSpawn(BaseCharacter owner, BaseWeapon parentWeapon)
    {
        this.owner = owner;
        this.parentWeapon = parentWeapon;
    }

    public void OnProjectileFullyCharged()
    {
        flashEffect?.Flash();
    }

    internal void OnProjectileRelease()
    {
        rb.isKinematic = false;
        // Detach the arrow from the bow
        transform.parent = null;
        // add impulse to the arrow
        rb.velocity = transform.up * speed;
        isFlying = true;
        // start coroutine to destroy the arrow after some time
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        flashEffect = GetComponent<SimpleFlashEffect>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isFlying)
        {
            return;
        }

        if (owner == null || parentWeapon == null)
        {
            return;
        }

        if (collision.gameObject.tag == owner.tag || collision.gameObject == parentWeapon.gameObject || !collideTags.Contains(collision.gameObject.tag))
        {
            return;
        }


        BaseCharacter hitTarget = collision.gameObject.GetComponent<BaseCharacter>();

        if (hitTarget != null)
        {
            DamageData damageData = parentWeapon.CalculateDamage(owner, hitTarget);
            hitTarget.TakeDamage(damageData);
        }

        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;
        transform.parent = collision.gameObject.transform;
        audioSource?.PlayOneShot(arrowHitSound);
        isFlying = false;

        if(trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }
}

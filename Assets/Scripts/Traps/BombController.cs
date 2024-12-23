using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SimpleFlashEffect))]
[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BombController : MonoBehaviour, IDamageable
{
    [SerializeField]
    private AudioClip explosionSound;
    [SerializeField]
    private AudioClip ignitedSound;
    [SerializeField]
    private AudioClip fuseBurningSound;
    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private float maxShakeMagnitude = 0.4f;
    [SerializeField]
    private float shakeDistanceThreshold = 10.0f;
    [SerializeField]
    private bool isIgnited = false;

    private Collider2D innerCol;
    private Collider2D outerCol;
    private bool isExploded = false;

    private PlayerController playerController;
    private SimpleFlashEffect flashEffect;
    private AudioSource audioSource;
    private Animator animator;
    private DamageSource damageSource;
    private Collider2D col;
    private Rigidbody2D rb;
    private Coroutine flashCoroutine;
    private ParticleSystem smokeParticle;

    private void Awake()
    {
        innerCol = transform.Find("Inner")?.GetComponent<Collider2D>();
        outerCol = transform.Find("Outer")?.GetComponent<Collider2D>();

        if (innerCol == null || outerCol == null)
        {
            Debug.LogError("BombController: Inner or Outer collider is missing");
        }

        playerController = GameObject.FindAnyObjectByType<PlayerController>();
        flashEffect = GetComponent<SimpleFlashEffect>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        damageSource = GetComponent<DamageSource>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        smokeParticle = GetComponentInChildren<ParticleSystem>();

        // freeze rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploded)
        {
            return;
        }

        if (isIgnited)
        {
            if (flashCoroutine == null)
            {
                flashCoroutine = StartCoroutine(FlashEffectCoroutine());
            }
        }

        animator.SetBool("IsIgnited", isIgnited);
    }

    public void IgniteBomb()
    {
        if (isIgnited)
        {
            return;
        }

        isIgnited = true;

        // play igniting sound
        if(ignitedSound)
        {
            audioSource.PlayOneShot(ignitedSound);
        }
    }

    private IEnumerator FlashEffectCoroutine()
    {
        while (!isExploded)
        {
            // play flash effect
            flashEffect.Flash();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void DealExplosionDamage()
    {
        // stop the flash effect coroutine
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        if(isExploded)
        {
            return;
        }

        if (explosionSound)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        isExploded = true;

        // Apply explosion damage to characters in the inner and outer radius
        ApplyExplosionDamage();

        // Disable the collider and rigidbody
        col.enabled = false;
        rb.linearVelocity = Vector2.zero;

        OnExplosionEffect();

        // Start coroutine to destroy the bomb
        StartCoroutine(DestroyBomb());
    }

    private void OnExplosionEffect()
    {
        playerController.ShakePlayerCamera(transform.position, shakeDuration, maxShakeMagnitude, shakeDistanceThreshold);
        // just for fun here we will check if there is any arrows as a child of the bomb
        // we will send them flying in a random direction
        // Go through every child of the bomb to find if there are any arrows
        // If there are, send them flying in a random direction
        foreach (Transform child in transform)
        {
            ArrowScript arrow = child.GetComponent<ArrowScript>();
            if (arrow != null)
            {
                Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
                // Re-enable the collider of the arrow
                // Detach the arrow from the bomb
                arrow.transform.parent = null;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
                arrow.GetComponent<Collider2D>().enabled = true;
                // change collider type not trigger
                arrow.GetComponent<Collider2D>().isTrigger = false;
                rb.linearVelocity = GetRandomDirection() * 10f;
                // Apply random rotation
                rb.freezeRotation = false;
                rb.angularVelocity = Random.Range(-360f, 360f);
            }
        }

        smokeParticle.Play();
    }

    private Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private IEnumerator DestroyBomb()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        List<Collider2D> innerVictims = GetCharactersInRadius(innerCol);
        List<Collider2D> outerVictims = GetCharactersInRadius(outerCol);

        foreach (Collider2D victim in innerVictims)
        {
            ApplyExplosionTo(true, victim);
        }

        foreach (Collider2D victim in outerVictims)
        {
            if (!innerVictims.Contains(victim))
            {
                ApplyExplosionTo(false, victim);
            }
        }
    }

    private List<Collider2D> GetCharactersInRadius(Collider2D radius)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<Collider2D> colliders = new List<Collider2D>();

        radius.Overlap(contactFilter, colliders);

        return colliders;
    }

    public void ApplyExplosionTo(bool isInInnerRadius, Collider2D victim)
    {
        DamageData damageData = null;
        if(isInInnerRadius)
        {
            damageSource.CriticalChance = 1f;
            damageSource.PushScale *= 1.2f;
            damageSource.Damage *= 2f;
            damageData = damageSource.GetDamageData(transform.position, victim.transform.position, true);
        }
        else
        {
            damageData = damageSource.GetDamageData(transform.position, victim.transform.position, true);
        }

        DamageUtils.TryToApplyDamageDataTo(gameObject, victim, damageData, damageSource, false);
        damageSource.ResetStats();
    }

    public void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        IgniteBomb();
    }

    public void TakeDirectEffectDamage(int amount, Effect effect, bool isInvisible = false)
    {
    }
}

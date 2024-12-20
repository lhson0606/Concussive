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
    private float maxShakeMagnitude = 0.1f;
    [SerializeField]
    private float shakeDistanceThreshold = 10.0f;
    [SerializeField]
    private bool isIgnited = false;

    private Collider2D innerCol;
    private Collider2D outerCol;
    private bool isExploded = false;

    private Camera playerCamera;
    private SimpleFlashEffect flashEffect;
    private AudioSource audioSource;
    private Animator animator;
    private DamageSource damageSource;
    private Collider2D col;
    private Rigidbody2D rb;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        innerCol = transform.Find("Inner")?.GetComponent<Collider2D>();
        outerCol = transform.Find("Outer")?.GetComponent<Collider2D>();

        if (innerCol == null || outerCol == null)
        {
            Debug.LogError("BombController: Inner or Outer collider is missing");
        }

        playerCamera = GameObject.FindAnyObjectByType<Camera>();
        flashEffect = GetComponent<SimpleFlashEffect>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        damageSource = GetComponent<DamageSource>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // freeze rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        // check if the camera is close enough to shake
        float distance = Vector2.Distance(transform.position, playerCamera.transform.position);
        if (distance <= shakeDistanceThreshold)
        {
            StartCoroutine(CameraShake(distance));
        }

        // just for fun here we will check if there is any arrows as a child of the bomb
        // we will send them flying in a random direction
        foreach (Transform child in transform)
        {
            ArrowScript arrow = child.GetComponent<ArrowScript>();
            if (arrow != null)
            {
                Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
                // re enable the collider of the arrow
                // detach the arrow from the bomb
                arrow.transform.parent = null;
                arrow.GetComponent<Collider2D>().enabled = true;
                rb.linearVelocity = GetRandomDirection() * 10f;
                // apply random rotation
                rb.freezeRotation = false;
                rb.angularVelocity = Random.Range(-360f, 360f);
            }
        }
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

    private IEnumerator CameraShake(float distance)
    {
        Vector3 originalPosition = playerCamera.transform.position;
        float elapsed = 0.0f;

        // Calculate shake magnitude based on distance
        float shakeMagnitude = maxShakeMagnitude * (1 - (distance / shakeDistanceThreshold));

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        playerCamera.transform.position = originalPosition;
    }

    private void ApplyExplosionDamage()
    {
        List<BaseCharacter> innerVictims = GetCharactersInRadius(innerCol);
        List<BaseCharacter> outerVictims = GetCharactersInRadius(outerCol);

        foreach (BaseCharacter victim in innerVictims)
        {
            ApplyExplosionTo(true, victim);
        }

        foreach (BaseCharacter victim in outerVictims)
        {
            if (!innerVictims.Contains(victim))
            {
                ApplyExplosionTo(false, victim);
            }
        }
    }

    private List<BaseCharacter> GetCharactersInRadius(Collider2D radius)
    {
        List<BaseCharacter> characters = new List<BaseCharacter>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<Collider2D> colliders = new List<Collider2D>();

        radius.Overlap(contactFilter, colliders);

        foreach (Collider2D collider in colliders)
        {
            BaseCharacter character = collider.GetComponent<BaseCharacter>();
            if (character != null)
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    public void ApplyExplosionTo(bool isInInnerRadius, BaseCharacter victim)
    {
        DamageData damageData = damageSource.GetDamageData(transform.position, victim.transform.position);
        if(isInInnerRadius)
        {
            damageData.Damage *= 2;
        }

        damageSource.AppyDamageDataTo(damageData, victim);
    }

    public void TakeDamage(DamageData damageData)
    {
        IgniteBomb();
    }
}

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class SpellShieldObj : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private AudioClip shieldHitSound;

    private GameObject owner;
    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource audioSource;
    private Coroutine shakeCoroutine;

    public GameObject GetOwner()
    {
        return owner;
    }
   
    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    private IEnumerator ShakeShield()
    {
        float time = 0f;
        Vector3 originalPos = transform.localPosition;
        while (time < shakeDuration)
        {
            transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * 0.1f;
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
        shakeCoroutine = null;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseProjectile projectile = collision.gameObject.GetComponent<BaseProjectile>();

        if(projectile != null)
        {
            if (projectile.GetOwner()?.tag == owner?.tag)
            {
                return;
            }

            if (audioSource && shieldHitSound)
            {
                audioSource.PlayOneShot(shieldHitSound);
            }

            if (shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeShield());
            }

            // Destroy the projectile
            Destroy(projectile.gameObject);
        }
    }
}
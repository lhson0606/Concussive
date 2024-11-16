using System;
using System.Collections;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField]
    private float speed = 16f;
    [SerializeField]
    private float timeToLive = 5f;

    private SimpleFlashEffect flashEffect;
    private Rigidbody2D rb;

    public void OnProjectileSpawn()
    {
        
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
        // start coroutine to destroy the arrow after some time
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flashEffect = GetComponent<SimpleFlashEffect>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

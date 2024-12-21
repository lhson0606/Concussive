using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ExpComponent : MonoBehaviour
{
    [SerializeField]
    private float movingSpeed = 300f;
    [SerializeField]
    private Rigidbody2D rb;


    private bool isCollected = false;
    private GameObject player;
    private Vector3 originalPosition;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float floatingSpeed = 2f;
        if (!isCollected)
        {
            // make it float around within floatingMagnitude
            rb.linearVelocity = new Vector2(Mathf.Sin(Time.time * floatingSpeed), Mathf.Cos(Time.time * floatingSpeed));
            return;
        }

        // move towards player, if we are collected
        if (isCollected)
        {
            Vector3 direction = player.transform.position - transform.position;
            rb.linearVelocity = direction.normalized * movingSpeed * Time.deltaTime;
        }

        //if we are close enough to the player, destroy the object
        if (Vector3.Distance(transform.position, player.transform.position) < 0.3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollected = true;
            StartCoroutine(DestroyExp());
        }
    }

    private IEnumerator DestroyExp()
    {
        // wait for 15 second before destroying the object
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }
}

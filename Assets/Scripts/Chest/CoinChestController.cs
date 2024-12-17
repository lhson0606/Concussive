using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class CoinChestController : MonoBehaviour
{
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    int coinCount = 4;
    [SerializeField]
    private float coinSpawnRadius = 0.5f;
    [SerializeField]
    private float coinSpawnForce = 5f;
    [SerializeField]
    private AudioClip chestOpenSound;

    private bool isOpened = false;
    private Animator animator;
    private Collider2D col;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("CoinChestController: No Animator component found on coinChestObj");
        }
        // Get the Collider2D component
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("CoinChestController: No Collider2D component found on coinChestObj");
        }
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("CoinChestController: No AudioSource component found on coinChestObj");
        }

        if(coinPrefab == null)
        {
            Debug.LogError("CoinChestController: coinPrefab is not set");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isOpened = true;
            UpdateState();
        }
    }

    void UpdateState()
    {
        if (isOpened)
        {
            animator.SetBool("isOpened", true);            
            col.enabled = false;
        }
    }

    // call from the animation event
    public void SpawnCoins()
    {
        if (chestOpenSound != null)
        {
            audioSource.PlayOneShot(chestOpenSound);
        }

        for (int i = 0; i < coinCount; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)transform.position + randomDirection * coinSpawnRadius;
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            CoinController coinController = coin.GetComponent<CoinController>();
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            rb.AddForce(randomDirection * coinSpawnForce, ForceMode2D.Impulse);
            coinController.SetShouldMove(false);

            // Start a coroutine to apply force to the coin and enable it after an interval
            StartCoroutine(EnableCoin(coinController, rb));
        }
    }

    private IEnumerator EnableCoin(CoinController coinController, Rigidbody2D rb)
    {
        yield return new WaitForSeconds(0.3f);
        coinController.SetShouldMove(true);
        rb.linearVelocity = Vector2.zero;
    }
}

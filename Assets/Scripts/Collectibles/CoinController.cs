using UnityEngine;

public class CoinController : MonoBehaviour
{
    
    [SerializeField]
    private float collectRadius = 4f;
    [SerializeField]
    private float collectSpeed = 8f;
    [SerializeField]
    bool isCollected = false;
    [SerializeField]
    AudioClip collectedSound = null;

    private int coinAmount = 1;

    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    private bool shouldMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coinAmount = Random.Range(1, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if(!shouldMove)
        {
            return;
        }

        if (DistanceToPlayer() < collectRadius && !isCollected)
        {
            isCollected = true;
            animator.SetBool("IsCollected", true);
        }

        if(isCollected)
        {
            rb.linearVelocity = DirectionToPlayer() * collectSpeed;
        }

        if(DistanceToPlayer() < 0.5f)
        {
            var playerController = player.GetComponent<PlayerController>();
            playerController.AddCoins(coinAmount);
            if(collectedSound != null)
            {
                playerController.PlaySound(collectedSound);
            }
            Destroy(gameObject);
        }
    }

    private float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }

    private Vector2 DirectionToPlayer()
    {
        return (player.transform.position - transform.position).normalized;
    }

    public void SetShouldMove(bool shouldMove)
    {
        this.shouldMove = shouldMove;
    }
}

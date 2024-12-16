using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PushableComponent : MonoBehaviour, IControlButtonInteractable
{
    [SerializeField]
    private float pushDuration = 1.0f; // Time required to push the box
    [SerializeField]
    private float moveDistance = 1.0f; // Distance to move the box (size of one grid cell)
    [SerializeField]
    private float lerpDuration = 0.5f; // Duration of the lerp for smooth movement
    [SerializeField]
    private LayerMask obstacleLayer; // Layer mask to specify which layers are considered obstacles
    [SerializeField]
    private AudioClip pushSound;

    private float pushTime = 0.0f;
    private bool isPushing = false;
    private bool isMoving = false;
    private Vector2 pushDirection;
    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource audioSource;    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        // Freeze rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isPushing && !isMoving)
        {
            pushTime += Time.deltaTime;
            if (pushTime >= pushDuration)
            {
                Vector2 targetPosition = rb.position + pushDirection * moveDistance;
                if (IsValidPosition(targetPosition))
                {
                    if(pushSound != null)
                    {
                        audioSource?.PlayOneShot(pushSound);
                    }
                    StartCoroutine(MoveBox(targetPosition));
                }
                pushTime = 0.0f;
                isPushing = false;
            }
        }
        else
        {
            pushTime = 0.0f;
        }
    }

    private bool IsValidPosition(Vector2 targetPosition)
    {
        // Create a ContactFilter2D to exclude the box's own collider
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(obstacleLayer);
        contactFilter.useTriggers = false;

        // Check if the target position overlaps with any obstacles
        Collider2D[] results = new Collider2D[2];
        int hitCount = Physics2D.OverlapBox(targetPosition, col.bounds.size, 0, contactFilter, results);
        return hitCount == 0 || (hitCount == 1 && results[0] == col);
    }

    private IEnumerator MoveBox(Vector2 targetPosition)
    {
        isMoving = true;

        Vector2 startPosition = rb.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < lerpDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / lerpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition); // Ensure the final position is set
        isMoving = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isMoving)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb == null)
            {
                return;
            }

            if (playerRb.velocity.magnitude < 0.00000001f)
            {
                isPushing = false;
                return;
            }

            Vector2 playerDirection = playerRb.velocity.normalized;
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 center = collision.collider.bounds.center;
            Vector2 direction = (contactPoint - center).normalized;


            //only push if playderdirection and direction are close to each other
            if (Vector2.Dot(playerDirection, direction) > 0.1f)
            {
                isPushing = false;
                return;
            }

            // Determine the dominant axis and set the push direction accordingly
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                pushDirection = new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                pushDirection = new Vector2(0, Mathf.Sign(direction.y));
            }

            pushDirection = pushDirection.normalized;
            isPushing = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPushing = false;
            pushTime = 0.0f;
        }
    }
}


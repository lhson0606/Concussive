using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class BaseThrownProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float targetHitThreshold = 1f; // Distance threshold to consider hitting the target
    [SerializeField]
    private float timeToLive = 5f;
    [SerializeField]
    private float gravity = -9.8f; // Gravity to simulate the parabolic path
    [SerializeField]
    private float spinSpeed = 360f; // Degrees per second
    [SerializeField]
    protected List<string> collideTags = new List<string> { "Wall" };

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 initialVelocity;
    private float elapsedTime = 0f;
    private bool isCollided = false;

    private AudioSource audioSource;

    private DamageSource damageSource;
    private BaseWeapon parentWeapon;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        damageSource = GetComponent<DamageSource>();
    }

    public void Launch(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        CalculateInitialVelocity();
        StartCoroutine(MoveInParabolicPath());
        Destroy(gameObject, timeToLive);
    }

    private void CalculateInitialVelocity()
    {
        startPosition = transform.position;
        Vector2 displacement = targetPosition - startPosition;
        float timeToTarget = displacement.magnitude / speed;
        initialVelocity = new Vector2(displacement.x / timeToTarget, (displacement.y - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget);
    }

    private IEnumerator MoveInParabolicPath()
    {
        while (!isCollided)
        {
            elapsedTime += Time.deltaTime;
            Vector2 position = startPosition + initialVelocity * elapsedTime + 0.5f * new Vector2(0, gravity) * elapsedTime * elapsedTime;
            transform.position = position;

            // Apply rotation to make the projectile spin
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            // Check if the projectile has reached the target position
            if (Vector2.Distance(transform.position, targetPosition) <= targetHitThreshold)
            {
                OnImpact(targetPosition);
                yield break;
            }

            yield return null;
        }
    }

    protected virtual void OnImpact(Vector3 impactPoint)
    {
        isCollided = true;
        // Implement the logic for hitting the ground or target
        // For now, just destroy the projectile on hitting the ground
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a line to visualize the path in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, transform.position);

        // Draw a sphere to visualize the target hit threshold
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPosition, targetHitThreshold);
    }

    public void SetParentWeapon(BaseWeapon parentWeapon)
    {
        this.parentWeapon = parentWeapon;
        damageSource = parentWeapon.GetDamageSource();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.isTrigger)
        //{
        //    return;
        //}

        if (!collideTags.Contains(collision.gameObject.tag))
        {
            return;
        }

        OnImpact(transform.position);
    }
}

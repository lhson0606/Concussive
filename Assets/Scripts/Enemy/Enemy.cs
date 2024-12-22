using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : BaseCharacter
{
    [SerializeField]
    protected string enemyName;
    [SerializeField]
    protected float chaseRadius = 8;
    [SerializeField]
    protected float attackRadius = 4;
    [SerializeField]
    protected float kiteRadius = 5;
    [SerializeField]
    protected bool isActivated = false;
    [SerializeField]
    protected AudioClip angryNoise;
    [SerializeField]
    protected float enemyAttackSpeed = 1f;
    [SerializeField]
    protected float aggression = 0.2f; // the higher the value, the more likely they won't kite away from target
    [SerializeField]
    protected float maxKitingDistance = 5f;

    protected event Action OnActivated;
    protected event Action OnDeactivated;
    protected GameObject player;
    protected BaseCharacter target;
    protected bool canSeeTarget = false;
    protected bool isTargetInAttackRange = false;
    protected DamageSource damageSource;
    protected bool shouldKiteAway = false;

    protected override void Awake()
    {
        base.Awake();
        animator = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        MovingToPosition = transform.position;
    }

    public override void Start()
    {
        base.Start();
        damageSource = GetPrimaryWeapon()?.GetDamageSource();
        if (damageSource != null)
        {
            damageSource.CoolDown = enemyAttackSpeed;
        }
    }

    public void SetTarget(BaseCharacter target) 
    {
        this.target = target;
    }

    internal void Activate()
    {
        isActivated = true;
        OnActivated?.Invoke();
        NotifyCanMoveStateChanged();
    }

    internal void Deactivate()
    {
        isActivated = false;
        OnDeactivated?.Invoke();
        NotifyCanMoveStateChanged();
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public void SafeAddActivationDelegate(Action action)
    {
        OnActivated -= action;
        OnActivated += action;
    }

    public void SafeRemoveActivationDelegate(Action action)
    {
        OnActivated -= action;
    }

    public void SafeAddDeactivationDelegate(Action action)
    {
        OnDeactivated -= action;
        OnDeactivated += action;
    }

    public void SafeRemoveDeactivationDelegate(Action action)
    {
        OnDeactivated -= action;
    }

    public BaseCharacter GetCurrentTarget()
    {
        return target;
    }

    public override void Update()
    {
        base.Update();

        if(!isActivated || IsFreezing || !CanMove())
        {
            return;
        }

        // If there is no target, check if the player is in sight
        // If the player is in sight, set the player as the target
        if (target == null)
        {
            if (IsGameObjectInSight(player))
            {
                SetTarget(player.GetComponent<BaseCharacter>());
            }
        }

        canSeeTarget = CanSeeTarget();
        if (!canSeeTarget && HasTarget())
        {
            if (!isTryingToResetTarget)
            {
                StartCoroutine(ResetTargetAfter(8f));
            }
        }

        if (canSeeTarget && HasTarget())
        {
            LookAtPosition = target.transform.position;
            // randomy play angry noise
            if (angryNoise != null && UnityEngine.Random.value < 0.0001f)
            {
                AudioUtils.PlayAudioClipAtPoint(angryNoise, transform.position);
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if (distance <= attackRadius)
            {
                isTargetInAttackRange = true;
                if (GetPrimaryWeapon()?.GetDamageSource().IsCoolDownReset() ?? false)
                {
                    AttackCurrentTarget();
                }
            } else
            {
                isTargetInAttackRange = false;
            }

            if(distance < kiteRadius)
            {
                shouldKiteAway = true;
            }
            else
            {
                shouldKiteAway = false;
            }
        }

        // randomly play idle sound
        if (idleSound != null && UnityEngine.Random.Range(0, 10000) < 1)
        {
            audioSource?.PlayOneShot(idleSound);
        }
    }

    public virtual void AttackCurrentTarget() 
    {
        LookAtPosition = target.transform.position;
        // damageSource?.ApplyCoolDown();
    }

    private bool isTryingToResetTarget = false;
    private IEnumerator ResetTargetAfter(float duration)
    {
        isTryingToResetTarget = true;
        yield return new WaitForSeconds(duration);

        if(!canSeeTarget)
        {
            target = null;
        }
        isTryingToResetTarget = false;
    }

    private bool IsGameObjectInSight(GameObject target)
    {
        if (target == null)
        {
            return false;
        }
        float distance = Vector3.Distance(target.transform.position, transform.position);
        // if the distance is too far away, return false
        if (distance > chaseRadius)
        {
            return false;
        }
        // check line of sight
        Vector2 direction = (player.transform.position - transform.position).normalized;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false; // Ignore trigger colliders
        contactFilter.SetLayerMask(Physics2D.AllLayers); // Check all layers
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int hitCount = Physics2D.Raycast(transform.position, direction, contactFilter, hits, distance);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.gameObject == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanSeeTarget()
    {
        if(target == null)
        {
            return false;
        }

        return IsGameObjectInSight(target.gameObject);
    }

    public bool HasTarget()
    {
        return target != null;
    }

    public bool IsTargetInSight()
    {
        return canSeeTarget;
    }

    internal bool IsTargetInAttackRange()
    {
        return isTargetInAttackRange;
    }

    public bool IsTargetInChaseRadius()
    {
        if(!HasTarget())
        {
            return false;
        }

        float distance = Vector3.Distance(target.transform.position, transform.position);
        return distance <= chaseRadius;
    }

    public bool IsAttackReset()
    {
        return damageSource != null && damageSource.IsCoolDownReset();
    }

    public float AttackRange
    {
        get
        {
            return attackRadius;
        }
    }

    public float ChaseRange
    {
        get
        {
            return chaseRadius;
        }
    }

    private void OnDrawGizmosSelected()
    {
        DebugUtils.DrawDebugCircle(transform.position, chaseRadius, Color.green);
        DebugUtils.DrawDebugCircle(transform.position, attackRadius, Color.red);
        DebugUtils.DrawDebugCircle(transform.position, kiteRadius, Color.blue);
    }

    internal float DistanceToCurrenTarget()
    {
        if (target == null)
        {
            return float.MaxValue;
        }
        return Vector3.Distance(target.transform.position, transform.position);
    }

    internal bool ShouldKiteAway()
    {
        return shouldKiteAway && UnityEngine.Random.value > aggression;
    }

    internal float GetAggression()
    {
        return aggression;
    }

    public Vector2 GetRadomKitingDirection()
    {
        Vector2 directionAwayFromTarget = (transform.position - target.transform.position).normalized;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

        // Ensure the random direction is biased away from the target
        float bias = 0.8f; // Adjust this value to control the bias strength (0.0 to 1.0)
        Vector2 kitingDirection = Vector2.Lerp(randomDirection, directionAwayFromTarget, bias).normalized;

        return kitingDirection;
    }

    public Vector2 GetRandomKitingPosition()
    {
        Vector2 kitingDirection = GetRadomKitingDirection();
        Vector2 kitingPosition = (Vector2)transform.position + kitingDirection * UnityEngine.Random.Range(3f, maxKitingDistance);
        return kitingPosition;
    }

    public Vector3 MovingToPosition;
}

using System;
using System.Collections;
using UnityEngine;

public class Enemy : BaseCharacter
{
    [SerializeField]
    protected string enemyName;
    [SerializeField]
    protected float chaseRadius;
    [SerializeField]
    protected float attackRadius = 8;
    [SerializeField]
    protected bool isActivated = false;
    [SerializeField]
    protected AudioClip angryNoise;

    protected event Action OnActivated;
    protected event Action OnDeactivated;
    protected GameObject player;
    protected BaseCharacter target;
    protected bool canSeeTarget = false;

    protected override void Awake()
    {
        base.Awake();
        animator = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Start()
    {
        base.Start();
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
                StartCoroutine(ResetTargetAfter(10f));
            }
        }

        if (canSeeTarget && HasTarget())
        {
            // randomy play angry noise
            if (angryNoise != null && UnityEngine.Random.value < 0.0001f)
            {
                AudioUtils.PlayAudioClipAtPoint(angryNoise, transform.position);
            }
        }

        // randomly play idle sound
        if (idleSound != null && UnityEngine.Random.Range(0, 10000) < 1)
        {
            audioSource?.PlayOneShot(idleSound);
        }
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
        if (distance > attackRadius)
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
}

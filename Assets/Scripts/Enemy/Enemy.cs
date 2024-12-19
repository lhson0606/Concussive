using System;
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

    private event Action OnActivated;
    private event Action OnDeactivated;

    protected GameObject player;

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

    public virtual void Attack() { }

    internal void Activate()
    {
        isActivated = true;
        OnActivated?.Invoke();
    }

    internal void Deactivate()
    {
        isActivated = false;
        OnDeactivated?.Invoke();
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
}

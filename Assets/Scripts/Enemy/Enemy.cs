using UnityEngine;

public class Enemy : BaseCharacter
{
    [SerializeField]
    protected string enemyName;
    [SerializeField]
    protected float chaseRadius;
    [SerializeField]
    protected float attackRadius = 8;

    protected GameObject player;

    public override void Start()
    {
        base.Start();
        animator = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void Attack() { }
}

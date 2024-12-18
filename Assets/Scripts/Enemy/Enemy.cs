using UnityEngine;

public class Enemy : BaseCharacter
{
    public string enemyName;
    public Transform target;
    public float chaseRadius;
    public float attackRadius = 8;
    public Transform homePosition;

    public override void Start()
    {
        base.Start();
        animator = this.GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void Attack() { }
}

using UnityEngine;

public class RangedEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();
    }

    public float AttackRange
    {
        get
        {
            return attackRadius;
        }
    }
}
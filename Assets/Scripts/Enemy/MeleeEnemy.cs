using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeEnemy : Enemy
{

    protected override void Awake()
    {
        base.Awake();
        BaseCharacter playerChar = player.GetComponent<BaseCharacter>();
        playerChar.OnDeath += OnPlayerDie;
    }

    public void FindTarget()
    {
        if (target == null)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) <= chaseRadius
            && Vector3.Distance(player.transform.position, this.transform.position) >= attackRadius)
            {
                target = player.GetComponent<BaseCharacter>();
            }
        }
    }


    public override void Update(){
        base.Update();
        if (!isActivated)
        {
            return;
        }

        FindTarget();

        if(target == null)
        {
            rb.linearVelocity = Vector2.zero;
        } else
        {
            if (damageSource.IsCoolDownReset())
            {
                RunToTarget();
            } else
            {
                rb.linearVelocity = GetRandomDirection() * runSpeed * Time.deltaTime;
            }
        }        
    }

    void RunToTarget()
    {
        if(!base.CanMove() || target == null)
        {
            return;
        }

        LookAtPosition = target.transform.position;
        if (Vector3.Distance(target.transform.position, this.transform.position) <= chaseRadius 
            && Vector3.Distance(target.transform.position, this.transform.position) >= attackRadius)
        {
            animator.SetBool("run", true);

            Vector3 direction = target.transform.position - this.transform.position;
            rb.linearVelocity = direction.normalized * runSpeed * Time.deltaTime;

        }
        else
        {
            target = null;
            animator.SetBool("run", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(target == null)
        {
            return;
        }

        if (col.gameObject == target.gameObject)
        {
            BaseCharacter target = col.gameObject.GetComponent<BaseCharacter>();
            TryToAttack(target);
        }
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
        return direction;
    }

    private void TryToAttack(BaseCharacter target)
    {
        if(damageSource.IsCoolDownReset())
        {
            damageSource.ApplyDamageTo(target, transform.position);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(player != null)
        {
            BaseCharacter playerChar = player.GetComponent<BaseCharacter>();
            if (playerChar != null)
            {
                playerChar.OnDeath -= OnPlayerDie;
            }
        }        
    }

    void OnPlayerDie()
    {
        animator.SetBool("run", false);
        rb.linearVelocity = Vector2.zero;
        target = null;
    }
}

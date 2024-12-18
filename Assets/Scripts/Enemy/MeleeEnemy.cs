using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeEnemy : Enemy
{
    private Transform target;

    public override void Start()
    {
        base.Start();
        target = player.transform;
        BaseCharacter playerChar = player.GetComponent<BaseCharacter>();
        playerChar.OnDeath += OnPlayerDie;
    }
    
    public void FixedUpdate(){
        CheckDistance();
    }
    void CheckDistance()
    {
        if(!base.CanMove() || target == null)
        {
            return;
        }

        if (Vector3.Distance(target.position, this.transform.position) <= chaseRadius 
            && Vector3.Distance(target.position, this.transform.position) >= attackRadius)
        {
            animator.SetBool("run", true);

            Vector3 direction = target.position - this.transform.position;

            base.LookAtPosition = direction;

            rb.linearVelocity = new Vector2(direction.normalized.x * runSpeed, direction.normalized.y * runSpeed);
        }
        else
        {
            animator.SetBool("run", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collider");
        if (col.gameObject.CompareTag("Player"))
            {
                IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
                BaseCharacter target = col.gameObject.GetComponent<BaseCharacter>();
                BaseCharacter owner = this.GetComponent<BaseCharacter>();

                BaseWeapon baseWeapon = this.GetComponent<BaseWeapon>();


                if (damageable != null)
                {
                    DamageData damageData = baseWeapon.CalculateDamage(owner, target);
                    damageable.TakeDamage(damageData);
                }
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

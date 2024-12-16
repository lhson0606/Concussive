using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeEnemy : Enemy
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
    
    public void FixedUpdate(){
        CheckDistance();
    }
    void CheckDistance()
    {
        if(!base.CanMove())
        {
            return;
        }

        if (Vector3.Distance(target.position, this.transform.position) <= chaseRadius 
            && Vector3.Distance(target.position, this.transform.position) >= attackRadius)
        {
            animator.SetBool("run", true);

            Vector3 direction = target.position - this.transform.position;

            base.LookAtPosition = direction;

            rb.velocity = new Vector2(direction.normalized.x * runSpeed, direction.normalized.y * runSpeed);
        }
        else
        {
            animator.SetBool("run", false);
            rb.velocity = Vector2.zero;
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
}

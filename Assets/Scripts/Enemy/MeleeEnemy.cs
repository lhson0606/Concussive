using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeEnemy : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is create

    // Update is called once per frame
    private Rigidbody2D rb;
    private float knockTime;
    public override void Update()
    {
        base.Update();
    }
    
    public void FixedUpdate(){
        CheckDistance();
    }
    void CheckDistance()
    {
        if (Vector3.Distance(target.position, this.transform.position) <= chaseRadius 
            && Vector3.Distance(target.position, this.transform.position) >= attackRadius)
        {
            animator.SetBool("run", true);

            Vector3 direction = target.position - this.transform.position;

            base.LookAtPosition = direction;

            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, runSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("run", false);
        }
    }


    public override void Die()
    {
        Destroy(this.gameObject);
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
    public override void TakeDamage(DamageData damageData)
    {
        rb = this.GetComponent<Rigidbody2D> ();
        rb.isKinematic = false;
        base.TakeDamage(damageData);
        StartCoroutine(KnockCo(rb));
    }

    private IEnumerator KnockCo(Rigidbody2D enemy)
    {
        yield return new WaitForSeconds(knockTime);
        enemy.velocity = Vector2.zero;
        enemy.isKinematic = true;
    }

}

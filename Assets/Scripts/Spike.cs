using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damage = 1;
    public float changedTime = 2.5f;
    private float hitTimer = 0f;
    private bool hit = false;
    private Animator animator;
    private bool aniState = true;
    private bool hitCooldown = false;
    private float hitCooldownTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetBool("Hit", aniState);
    }

    // Update is called once per frame
    void Update()
    {

        hitTimer += Time.deltaTime; 
        if (hitTimer >= changedTime)
        {
            hitTimer = 0f; 
            aniState = !aniState;
            animator.SetBool("Hit", aniState);
        }

        if (hitCooldown)
        {
            hitCooldownTime -= Time.deltaTime;
            if (hitCooldownTime <= 0)
            {
                hitCooldownTime = 1f;
                hitCooldown = false;
            }
        }
        
    }

    void OnTriggerStay2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && hit && !hitCooldown)
            {
                IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
                BaseCharacter target = col.gameObject.GetComponent<BaseCharacter>();


                if (damageable != null)
                {
                    DamageData damageData = new DamageData();
                    damageData.SourceElement = this.gameObject.GetComponent<Element>();
                    damageData.Damage = damage;
                    damageData.SourcePosition = this.transform.position;
                    damageData.TargetPosition = target.transform.position;
                    damageable.TakeDamage(damageData);
                    hitCooldown = true;
                }
            }
    }

    private void switchHit()
    {
        hit = !hit;
    }

}

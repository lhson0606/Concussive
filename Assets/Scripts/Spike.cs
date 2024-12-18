using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damage = 1;
    public float changedTime = 2.0f;
    private float hitTimer = 0f;
    [SerializeField]
    private bool hit = true;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetBool("Hit", hit);
    }

    // Update is called once per frame
    void Update()
    {
        hitTimer += Time.deltaTime; 
        if (hitTimer >= changedTime)
        {
            hit = !hit;
            hitTimer = 0f; 
            animator.SetBool("Hit", hit);
        }
        
    }

    void OnTriggerStay2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && hit)
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
                }
            }
    }
}

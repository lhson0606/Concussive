using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damage = 1;
    public float changedTime = 2.5f;
    private float hitTimer = 0f;
    private bool hit = false;
    private Animator animator;
    private bool aniState = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private DamageSource damageSource;
    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetBool("Hit", aniState);
        damageSource = this.GetComponent<DamageSource>();
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
        
    }

    void OnTriggerStay2D (Collider2D col)
    {   
        BaseCharacter target = col.gameObject.GetComponent<BaseCharacter>();
        if (hit && damageSource.IsCoolDownReset() && target != null)
            {
                damageSource.ApplyDamageTo(target, this.gameObject.transform.position, true);
            }
    }
    
    public void switchHit()
    {
        hit = !hit;
    }


}

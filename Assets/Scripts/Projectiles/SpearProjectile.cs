using UnityEngine;

public class SpearProjectile : BaseProjectile
{
    [SerializeField]
    private float speed = 10f;

    private DamageSource damageSource;
    private Vector2 direction;
    private Collider2D col;
    private BaseWeapon parentWeapon;
    private GameObject owner;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    internal void Launch()
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    internal void SetDamageSource(DamageSource damageSource)
    {
        this.damageSource = damageSource;
    }

    internal void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    internal void SetParentWeapon(BaseWeapon parentWeapon)
    {
        this.parentWeapon = parentWeapon;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return;
        }

        // Ensure the collision is not with the damage source owner or the parent weapon
        if (damageSource == null ||
            damageSource.Owner?.tag == collision.gameObject.tag ||
            ReferenceEquals(collision.gameObject, parentWeapon.gameObject) ||
            ReferenceEquals(collision.gameObject, damageSource.Owner))
        {
            return;
        }
        DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource, false);
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        transform.parent = collision.gameObject.transform;
        //disable the collider
        col.enabled = false;
    }
}

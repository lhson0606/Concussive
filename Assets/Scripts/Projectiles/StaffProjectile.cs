using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class StaffProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    private DamageSource damageSource;
    private Vector2 direction;
    private Rigidbody2D rb;
    private Collider2D col;
    private BaseWeapon parentWeapon;

    public void Awake()
    {
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
        if(!damageSource || damageSource.Owner?.tag == collision.gameObject.tag || collision.gameObject == parentWeapon.gameObject)
        {
            return;
        }

        if(collision.isTrigger)
        {
            return;
        }

        Debug.Log($"StaffProjectile: OnTriggerEnter2D: {collision.gameObject.name}");

        DamageUtils.TryToApplyDamageTo(damageSource.Owner, collision, damageSource, false);
        Destroy(gameObject);
    }
}

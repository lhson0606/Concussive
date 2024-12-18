using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponControl : SlowMotionObject
{
    public float stopAimingDistance = 0.5f;
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 PointerPosition { get; set; }
    private BaseCharacter character;
    public bool ShouldAlterRenderOrder { get; set; } = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = transform.parent.GetComponent<BaseCharacter>();
        UpdateWeaponRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPointerTooClose())
        {
            return;
        }

        if(character.IsAttacking)
        {
            return;
        }

        UpdateWeaponRotation();
        UpdateWeaponRenderOrder();
    }

    private bool IsPointerTooClose()
    {
        return Vector2.Distance(transform.position, PointerPosition) < stopAimingDistance;
    }

    private void UpdateWeaponRenderOrder()
    {
        if(!ShouldAlterRenderOrder)
        {
            return;
        }

        if (characterRenderer == null)
        {
            return;
        }

        if(weaponRenderer == null)
        {
            return;
        }

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    void UpdateWeaponRotation()
    {
        if (characterRenderer == null)
        {
            return;
        }

        if (weaponRenderer == null)
        {
            return;
        }

        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        if(character==null || character.LookDir == null)
        {
            return;
        }
        if(character.LookDir.x<0)
        {
            transform.right = -direction;
        }else if(character.LookDir.x > 0)
        {
            transform.right = direction;
        }


        //Vector2 scale = transform.localScale;
        //if (direction.x < 0)
        //{
        //    scale.y = -1;
        //}
        //else if(direction.x > 0)
        //{
        //    scale.y = 1;
        //}

        //transform.localScale = scale;
    }
}

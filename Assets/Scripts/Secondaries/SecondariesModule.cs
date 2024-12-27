using System;
using System.Buffers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecondariesModule : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> secondarySlots = new List<GameObject>();

    private List<BaseWeapon> secondaries = new List<BaseWeapon>();
    private BaseCharacter owner;
    private bool isActivated = false;

    public void Awake()
    {
        SpawnSecondaries();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!owner)
        {
            return;
        }

        if(isActivated)
        {
            AutoAim();
            Fire();
        }       
    }

    private void AutoAim()
    {
        Vector2 TargetPosition = owner.LookAtPosition;

        Vector2 direction = (TargetPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }

    private void Fire()
    {
        foreach (BaseWeapon secondary in secondaries)
        {
            if (secondary.gameObject.GetComponent<DamageSource>().IsCoolDownReset())
            {
                secondary.DoAttack();
            }
        }
    }

    public void AimAndFireWithProbability(float probability)
    {
        AutoAim();
        foreach (BaseWeapon secondary in secondaries)
        {
            DamageSource damageSource = secondary.gameObject.GetComponent<DamageSource>();
            if (damageSource.IsCoolDownReset())
            {
                if(UnityEngine.Random.value < probability)
                {
                    secondary.DoAttack();
                }
                else
                {
                    secondary.GetDamageSource().ApplyCoolDown();
                }
            }
        }
    }

    public void SetOwner(BaseCharacter owner)
    {
        this.owner = owner;
        foreach (BaseWeapon secondary in secondaries)
        {
            secondary.SetUpAsAutoSecondary(owner);
        }
    }

    private void SpawnSecondaries()
    {
        foreach(GameObject secondarySlot in secondarySlots)
        {
            GameObject secondary = Instantiate(secondarySlot, transform.position, Quaternion.identity, gameObject.transform);
            BaseWeapon secondaryWeapon = secondary.GetComponent<BaseWeapon>();

            if(secondaryWeapon == null)
            {
                Debug.LogError("Secondary weapon is missing BaseWeapon component");
                return;
            }

            secondaries.Add(secondaryWeapon);
            // make it invisible
            // secondary.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
    }
}

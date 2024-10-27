using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class BaseCharacter : SlowMotionObject
{
    public int maxHealth = 7;
    public int maxArmor = 5;
    public int maxMana = 5;
    public int runSpeed = 240;
    public float criticalHitChance = 0.1f;
    public float criticalHitMultiplier = 2.0f;

    protected List<Effect> effects = new List<Effect>();

    private int currentHealth;
    private int currentArmor;
    private int currentMana;

    public void Start()
    {
        currentHealth = 1;
        currentArmor = maxArmor;
        currentMana = maxMana;
    }

    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
    }

    public void RemoveEffect(Effect effect)
    {
        effects.Remove(effect);
    }

    public void Heal(int healingAmount)
    {
        Debug.Log("Healing " + healingAmount + " health");
        if (currentHealth + healingAmount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += (int)healingAmount;
        }
    }
}

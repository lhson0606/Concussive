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
    public GameObject characterText = null;

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

    public void Heal(int healingAmount, EffectType healingType = EffectType.HEALING)
    {
        Debug.Log("Healing " + healingAmount + " health");
        if (currentHealth + healingAmount > maxHealth)
        {
            healingAmount = 0;
        }

        currentHealth += healingAmount;

        String text = "+" + healingAmount + " HP";
        Color textColor = EffectConfig.Instance.GetEffectTextColor(healingType);
        float lifeTime = 0.8f;
        Vector2 initVel = new Vector2(0, 1);
        this.SpawnText(text, textColor, lifeTime, initVel);
    }

    public void SpawnText(string text, Color color, float lifeTime, Vector2 initVel)
    {
        if (characterText == null)
        {
            Debug.LogError("CharacterText prefab is not assigned.");
            return;
        }

        GameObject textObject = Instantiate(characterText, transform.position, Quaternion.identity, gameObject.transform);
        PopUpTextControl textControl = textObject.GetComponent<PopUpTextControl>();

        if (textControl == null)
        {
            Debug.LogError("PopUpTextControl component not found on characterText prefab.");
            return;
        }

        textControl.text = text;
        textControl.textColor = color;
        textControl.lifeTime = lifeTime;
        textControl.InitialVelocity = initVel;
    }
}

using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    [SerializeField]
    protected string skillName = "?";
    [SerializeField]
    protected GameObject flashScreenPrefab;
    [SerializeField]
    private float cooldown = 10f;

    private float cooldownTimer = 0f;

    protected GameObject owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Use()
    {
        if(IsReady())
        {
            ResetCooldown();
            OnUse();
        }
    }

    public virtual void OnUse()
    {
        Debug.Log("Skill used");
    }

    public virtual void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool IsReady()
    {
        return cooldownTimer <= 0;
    }

    public void ResetCooldown()
    {
        cooldownTimer = cooldown;
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }
}

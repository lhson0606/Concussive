using UnityEngine;
using UnityEngine.Rendering;

public class BowScript : BaseWeapon
{
    [SerializeField]
    private GameObject arrowPrefab;

    [Range(0, 3)]
    private int chargeLevel = 0;

    private bool isCharging = false;
    private float chargeTime = 0.0f;

    protected override void Start()
    {
        base.Start();
        base.ShouldAlterRenderOrder = false;
    }

    public override void DoAttack()
    {
        isCharging = true;
        Debug.Log("Charging attack");
    }

    public override void SetAsMainWeapon(BaseCharacter owner)
    {
        base.SetAsMainWeapon(owner);
        base.ShouldAlterRenderOrder = false;
        // set render order to character +1
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
    }

    public override void Update()
    {
        base.Update();
        if (isCharging)
        {
            chargeTime += Time.deltaTime;

            chargeLevel = GetClampedIntParameter(chargeTime, 0.0f, 1.0f, 2.0f, 3);

            Debug.Log($"Charging at level {chargeLevel}");
        }
    }

    public override void ReleaseAttack()
    {
        isCharging = false;
        chargeTime = 0.0f;
        chargeLevel = 0;
        Debug.Log("Releasing attack");
    }

    private int GetClampedIntParameter(float time, float level1, float level2, float level3, int maxLevel)
    {
        if (time >= level3)
            return maxLevel;
        if (time >= level2)
            return 2;
        if (time >= level1)
            return 1;
        return 0;
    }
}

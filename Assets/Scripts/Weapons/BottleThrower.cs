using UnityEngine;

public class BottleThrower : BaseWeapon
{
    [SerializeField]
    private GameObject bottlePrefab;
    [SerializeField]
    private AudioClip onThrowSound;

    protected override void Start()
    {
        base.Start();
        base.ShouldAlterRenderOrder = false;
    }

    public override void DoAttack()
    {
        base.DoAttack();

        if (!damageSource.IsCoolDownReset())
        {
            return;
        }

        Vector2 throwDestination = owner.LookAtPosition;
        //spawn bottle
        GameObject bottle = Instantiate(bottlePrefab, transform.position, Quaternion.identity);
        ThrowableBottle bottleScript = bottle.GetComponent<ThrowableBottle>();
        bottleScript.SetParentWeapon(this);
        bottleScript.Launch(throwDestination);
        damageSource.ApplyCoolDown();
    }
}

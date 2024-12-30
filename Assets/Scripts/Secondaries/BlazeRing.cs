using UnityEngine;

public class BlazeRing : BaseWeapon
{
    public override void OnSetOwner(BaseCharacter owner)
    {
        // get all WispFire components in children
        WispFire[] wispFires = GetComponentsInChildren<WispFire>();
        foreach (WispFire wispFire in wispFires)
        {
            wispFire.SetOwner(owner);
        }
    }
}

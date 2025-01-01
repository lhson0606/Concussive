using UnityEngine;

public class BlazeScript : RangedEnemy
{
    private SecondariesModule secondariesModule;

    public override void Start()
    {
        base.Start();
        secondariesModule = GetComponentInChildren<SecondariesModule>();
        secondariesModule.SetOwner(this);
    }
}

using System;
using UnityEngine;

public class ThrowableBottle : BaseThrownProjectile
{
    [SerializeField]
    private AudioClip impactSound;
    [SerializeField]
    private GameObject poisonousAreaPrefab;

    protected override void OnImpact(Vector3 impactPoint)
    {
        base.OnImpact(impactPoint);
        if(impactSound != null)
        {
            AudioUtils.PlayAudioClipAtPoint(impactSound, impactPoint);
        }
        if (poisonousAreaPrefab != null)
        {
            GameObject poisonousArea = Instantiate(poisonousAreaPrefab, impactPoint, Quaternion.identity);
            PoisonousArea poisonousAreaScript = poisonousArea.GetComponent<PoisonousArea>();
        }
    }
}

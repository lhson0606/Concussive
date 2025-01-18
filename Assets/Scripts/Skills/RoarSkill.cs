using UnityEngine;

public class RoarSkill : BaseSkill
{
    [SerializeField]
    private AudioClip roarSound;
    private GlobalShockWaveController shockWaveController;

    private void Awake()
    {
        shockWaveController = FindFirstObjectByType<GlobalShockWaveController>();
    }

    public override void OnUse()
    {
        base.OnUse();
        shockWaveController.DoShockWave(owner.transform.position, 1f, 1f);
        if(roarSound != null)
            AudioUtils.PlayAudioClipAtPoint(roarSound, owner.transform.position);
        owner.GetComponent<BaseCharacter>().Heal(64);
    }
}

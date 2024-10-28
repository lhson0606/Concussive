using System;
using UnityEngine;

public class BasePotion : GameItem
{
    [SerializeField]
    private AudioClip usingAudio;
    public Effect effectPrefab;

    protected new void Start()
    {
        base.Start();
    }

    protected virtual void OnUsing(BaseCharacter baseCharacter)
    {
        
    }

    protected override void OnPickUp(BaseCharacter baseCharacter)
    {
        Debug.Log("Potion used");
        PlayUsingSound();
        Effect effectInstance = Instantiate(effectPrefab, baseCharacter.transform);
        effectInstance.StartEffect(baseCharacter);
        OnUsing(baseCharacter);
        // Destroy the potion after spawning the effect
        DestroyPotion();
    }

    private void PlayUsingSound()
    {
        if (playerController != null && usingAudio != null)
        {
            playerController.PlaySound(usingAudio);
        }
    }

    protected void DestroyPotion()
    {
        Destroy(gameObject);
    }
}

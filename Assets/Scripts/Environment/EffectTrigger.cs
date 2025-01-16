using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Collider2D))]
public class EffectTrigger : MonoBehaviour
{
    [SerializeField]
    private Effect effectPrefab; // Reference to the effect prefab
    [SerializeField]
    private bool destroyAfterTrigger = false;
    [SerializeField]
    private float cooldown = 1f;

    private float lastTriggerTime = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyEffect(collision.gameObject);
        }
    }

    private void ApplyEffect(GameObject player)
    {
        if (Time.time - lastTriggerTime < cooldown)
        {
            return;
        }

        if (effectPrefab != null)
        {
            BaseCharacter baseCharacter = player.GetComponent<BaseCharacter>();

            if (baseCharacter == null)
            {
                Debug.LogWarning("BaseCharacter component not found on the player.");
                return;
            }

            // Instantiate the effect prefab at the player's position
            Effect effectInstance = Instantiate(effectPrefab, baseCharacter.transform.position, Quaternion.identity, baseCharacter.transform);
            effectInstance.StartEffect(baseCharacter);

            lastTriggerTime = Time.time;

            if (destroyAfterTrigger)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Effect prefab is not assigned.");
        }
    }
}

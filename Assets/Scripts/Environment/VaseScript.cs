using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VaseScript : MonoBehaviour, IDamageable
{
    [SerializeField]
    List<GameObject> itemsToSpawn = new List<GameObject>();
    [SerializeField]
    AudioClip breakSound;

    public void TakeDamage(DamageData damageData, bool isInvisible = false)
    {
        Break();
    }

    public void TakeDirectEffectDamage(int amount, Effect effect, bool ignoreArmor = false, bool isInvisible = false)
    {

    }

    void Break()
    {
        if(breakSound != null)
        {
            AudioUtils.PlayAudioClipAtPoint(breakSound, transform.position, 0.5f);
        }

        SpawnItems();

        Destroy(gameObject);
    }

    private void SpawnItems()
    {
        foreach (GameObject item in itemsToSpawn)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}

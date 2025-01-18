using System.Collections.Generic;
using UnityEngine;

public class SummonHomies : BaseSkill
{
    [SerializeField]
    private List<GameObject> homies = new List<GameObject>();
    [SerializeField]
    private int summonCount = 4;
    [SerializeField]
    private float summonRadius = 16f;
    [SerializeField]
    private float summonDelay = 1.5f;
    [SerializeField]
    private int aliveHomiesLimit = 10;

    private List<HomiesSpawner> spawners = new List<HomiesSpawner>();
    private int aliveHomies = 0;

    public override void OnUse()
    {
        if(!owner)
        {
            Debug.LogError("Owner is not set");
        }

        spawners.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(Physics2D.AllLayers); // Check all layers
        contactFilter.useTriggers = true; // Include trigger colliders
        contactFilter.useLayerMask = true; // Use the layer mask
        contactFilter.useDepth = false; // Do not filter by depth

        // Get all colliders within the radius
        List<Collider2D> colliders = new List<Collider2D>(20);
        Physics2D.OverlapCircle(owner.transform.position, summonRadius, contactFilter, colliders);

        // Filter the colliders to get HomiesSpawner components
        foreach (Collider2D collider in colliders)
        {
            HomiesSpawner spawner = collider.GetComponent<HomiesSpawner>();
            if (spawner != null)
            {
                spawners.Add(spawner);
            }
        }

        // spawn homies at spawners
        int homiesToSummon = summonCount - aliveHomies;
        for (int i = 0; i < homiesToSummon; i++)
        {
            if (spawners.Count == 0)
            {
                break;
            }
            // Get a random spawner
            int randomIndex = Random.Range(0, spawners.Count);
            HomiesSpawner spawner = spawners[randomIndex];
            Vector3 spawnPosition = spawner.transform.position;
            // Instantiate a random homie prefab
            GameObject homiePrefab = homies[Random.Range(0, homies.Count)];
            GameObject homie = Instantiate(homiePrefab, spawnPosition, Quaternion.identity);
            Enemy homieCharacter = homie.GetComponent<Enemy>();
            //increment alive homies
            aliveHomies++;

            //activate homie
            if(homieCharacter != null)
            {
                homieCharacter.Activate();
            }

            // register on death event
            homieCharacter.SafeDelegateOnDeath(() =>
            {
                aliveHomies--;
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the summon radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, summonRadius);
    }
}

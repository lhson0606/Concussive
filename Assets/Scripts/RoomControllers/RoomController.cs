using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomController : MonoBehaviour
{
    private Collider2D col;
    private RoomTrigger roomTrigger;

    private List<BarrierController> barriers = new List<BarrierController>();
    private List<BaseCharacter> enemies = new List<BaseCharacter>();

    private int enemiesDefeated = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        roomTrigger = GetComponentInChildren<RoomTrigger>();

        List<Collider2D> colliders = new List<Collider2D>();
        ScanForColliders(colliders);

        ScanBarriers(colliders);
        ScanForEnemies(colliders);

        SetUpEnemies();

        // set up delegates
        roomTrigger.OnPlayerEnterRoom -= OnPlayerEnterRoom;
        roomTrigger.OnPlayerEnterRoom += OnPlayerEnterRoom;
        OpenBarriers();
    }

    private void SetUpEnemies()
    {
        // add OnDie delegate to each enemy
        foreach (BaseCharacter enemy in enemies)
        {
            enemy.OnDeath -= OnEnemyDie;
            enemy.OnDeath += OnEnemyDie;
        }
    }

    private void ScanBarriers(List<Collider2D> colliders)
    {
        foreach (Collider2D collider in colliders)
        {
            BarrierController barrier = collider.GetComponent<BarrierController>();
            if (barrier != null)
            {
                barriers.Add(barrier);
            }
        }
    }

    private void ScanForEnemies(List<Collider2D> colliders)
    {
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseCharacter enemy = collider.GetComponent<BaseCharacter>();
                if (enemy != null)
                {
                    enemies.Add(enemy);
                }
            }
        }
    }

    private void ScanForColliders(List<Collider2D> colliders)
    {
        // Create a ContactFilter2D to filter the colliders
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(Physics2D.AllLayers); // Check all layers
        contactFilter.useTriggers = true; // Include trigger colliders
        contactFilter.useLayerMask = true; // Use the layer mask
        contactFilter.useDepth = false; // Do not filter by depth

        // Find all colliders that overlap with the RoomController's collider
        col.Overlap(contactFilter, colliders);

        // Manually add disabled colliders
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>(true); // Include inactive objects
        foreach (Collider2D collider in allColliders)
        {
            if (!collider.enabled && col.bounds.Intersects(collider.bounds))
            {
                colliders.Add(collider);
            }
        }
    }

    private void CloseBarriers()
    {
        foreach (BarrierController barrier in barriers)
        {
            barrier.Close();
        }
    }

    private void OpenBarriers()
    {
        foreach (BarrierController barrier in barriers)
        {
            barrier.Open();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerEnterRoom()
    {
        CloseBarriers();
        foreach (BaseCharacter enemy in enemies)
        {
            enemy.Activate();
        }
    }

    void OnEnemyDie()
    {
        enemiesDefeated++;
        if (enemiesDefeated == enemies.Count)
        {
            OpenBarriers();
        }
    }

    void OnDestroy()
    {
        foreach (BaseCharacter enemy in enemies)
        {
            enemy.OnDeath -= OnEnemyDie;
        }
    }
}
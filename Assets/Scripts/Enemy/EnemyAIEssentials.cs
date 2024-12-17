using System;
using Unity.Behavior;
using UnityEngine;

public class EnemyAIEssentials : MonoBehaviour
{
    BaseCharacter character;
    BehaviorGraphAgent behaviorGraphAgent;
    Blackboard blackboard;
    GameObject player;
    private void Awake()
    {
        character = GetComponent<BaseCharacter>();
        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        if(behaviorGraphAgent == null)
        {
            Debug.LogError("behaviorGraphAgent is missing on " + gameObject.name);
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character.OnActivated += OnActivated;
        character.OnDeactivated += OnDeactivated;

        behaviorGraphAgent.SetVariableValue<bool>("IsActivated", character.IsActivated());
    }

    // Update is called once per frame
    void Update()
    {
        if(!character.IsActivated())
        {
            return;
        }

        bool canSeePlayer = CanSeePlayer();
        behaviorGraphAgent.SetVariableValue<bool>("CanSeePlayer", canSeePlayer);
        if(canSeePlayer)
        {
            character.LookAtPosition = player.transform.position;
        }
    }

    private bool CanSeePlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        // check distance
        if (distance > 10)
        {
            return false;
        }

        // check line of sight
        Vector2 direction = (player.transform.position - transform.position).normalized;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false; // Ignore trigger colliders
        contactFilter.SetLayerMask(Physics2D.AllLayers); // Check all layers

        RaycastHit2D[] hits = new RaycastHit2D[2];
        int hitCount = Physics2D.Raycast(transform.position, direction, contactFilter, hits, distance);

        if(hits.Length > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.gameObject.tag == "Player")
                {
                    return true;
                }
            }
        }

        return false;
    }


    void OnActivated()
    {
        behaviorGraphAgent.SetVariableValue<bool>("IsActivated", true);
    }

    void OnDeactivated()
    {
        behaviorGraphAgent.SetVariableValue<bool>("IsActivated", false);
    }

    private void OnDestroy()
    {
        character.OnActivated -= OnActivated;
        character.OnDeactivated -= OnDeactivated;
    }
}

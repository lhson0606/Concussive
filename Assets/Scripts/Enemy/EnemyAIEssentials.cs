using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIEssentials : MonoBehaviour
{
    Enemy character;
    BehaviorGraphAgent behaviorGraphAgent;
    Blackboard blackboard;
    GameObject player;
    NavMeshAgent navMeshAgent;
    Rigidbody2D rb;
    private void Awake()
    {
        character = GetComponent<Enemy>();
        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        // fix rotation issues since we are using 2D
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        // check if agent is a ranged enemy (RangedEnemy)
        RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
        if(rangedEnemy != null)
        {
            navMeshAgent.stoppingDistance = rangedEnemy.AttackRange;
        }

        if (behaviorGraphAgent == null)
        {
            Debug.LogError("behaviorGraphAgent is missing on " + gameObject.name);
        }
        
        player = GameObject.FindGameObjectWithTag("Player");

        character.SafeAddActivationDelegate(OnActivated);
        character.SafeAddDeactivationDelegate(OnDeactivated);
        character.SafeDelegateOnHurt(OnHurt);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorGraphAgent.SetVariableValue<bool>("IsAdctivated", character.IsActivated());
    }

    Vector2 lastAgentPos = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        if(!character.IsActivated() || !character.CanMove())
        {
            return;
        }

        bool canSeePlayer = CanSeePlayer();
        behaviorGraphAgent.SetVariableValue<bool>("CanSeePlayer", canSeePlayer);
        if(canSeePlayer)
        {
            character.LookAtPosition = player.transform.position;
            behaviorGraphAgent.SetVariableValue<Vector2>("PlayerLastPosition", player.transform.position);
            behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", false);
        }

        //// testing
        //if (canSeePlayer)
        //{
        //    navMeshAgent.SetDestination(player.transform.position);

        //    // move towards the player
        //    if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        //    {
        //        Vector2 direction = (player.transform.position - transform.position).normalized;
        //        rb.linearVelocity = direction * character.GetRunSpeed() * Time.deltaTime;
        //        // draw debug line
        //        Debug.DrawRay(transform.position, direction * 10, Color.red);
        //        // stop the agent after a while
        //        StartCoroutine(StopAgent());
        //    }
        //}
    }

    private IEnumerator StopAgent()
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;
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
        character.SafeRemoveActivationDelegate(OnActivated);
        character.SafeRemoveDeactivationDelegate(OnDeactivated);
        character.RemoveDelegateOnHurt(OnHurt);
    }

    public void OnHurt(DamageData damage)
    {
        if (!character.IsActivated())
        {
            character.Activate();
        }

        // check the direction of the damage
        behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", false);
        behaviorGraphAgent.SetVariableValue<Vector2>("PlayerLastPosition", damage.SourcePosition);
    }
}

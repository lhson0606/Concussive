using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIEssentials : MonoBehaviour
{
    Enemy character;
    BehaviorGraphAgent behaviorGraphAgent;
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

        character.SafeAddActivationDelegate(OnActivated);
        character.SafeAddDeactivationDelegate(OnDeactivated);
        character.SafeDelegateOnHurt(OnHurt);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorGraphAgent.SetVariableValue<bool>("IsActivated", character.IsActivated());
    }

    Vector2 lastAgentPos = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        if(!character.IsActivated() || !character.CanMove())
        {
            return;
        }

        behaviorGraphAgent.SetVariableValue<bool>("CanSeeTarget", character.IsTargetInSight());
        
        if(character.IsTargetInSight())
        {
            behaviorGraphAgent.SetVariableValue<Vector2>("TargetLastPosition", character.GetCurrentTarget().transform.position);
            behaviorGraphAgent.SetVariableValue<bool>("CheckedTargetLastPosition", false);
        }

        behaviorGraphAgent.SetVariableValue<bool>("IsInRange", character.IsTargetInAttackRange());
        behaviorGraphAgent.SetVariableValue<bool>("IsInChaseRadius", character.IsTargetInChaseRadius());
        behaviorGraphAgent.SetVariableValue<bool>("AttackReset", character.IsAttackReset());


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

        if(damage.DamageDealer != null)
        {
            BaseCharacter baseCharacter = damage.DamageDealer.GetComponent<BaseCharacter>();
            if (baseCharacter != null)
            {
                character.SetTarget(baseCharacter);
            }
            character.SetTarget(baseCharacter);
        }
        
        // check the direction of the damage
        behaviorGraphAgent.SetVariableValue<bool>("CheckedTargetLastPosition", false);
        behaviorGraphAgent.SetVariableValue<Vector2>("TargetLastPosition", damage.SourcePosition);
    }
}

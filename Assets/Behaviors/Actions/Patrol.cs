using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Agent searches for random places or last-seen player position", category: "Action", id: "b4277f60e0a7188e5162f27082dad714")]
public partial class PatrolAction : Action
{
    private Enemy entity;
    private NavMeshAgent navMeshAgent;
    private BehaviorGraphAgent behaviorGraphAgent;
    private Vector2 checkingPosition;
    private bool isPatrolling = false;
    private Rigidbody2D rb;
    private float stoppingDistance = 0.5f;
    private float patrolTime = 5f;
    // if it takes too long to reach the destination, change the destination
    private float patrolTimer = 0f;

    protected override Status OnStart()
    {
        entity = GameObject.GetComponent<Enemy>();

        if (entity == null)
        {
            Debug.LogError("Enemy component is missing on " + GameObject.name);
            return Status.Failure;
        }

        navMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on " + GameObject.name);
            return Status.Failure;
        }

        behaviorGraphAgent = GameObject.GetComponent<BehaviorGraphAgent>();
        if (behaviorGraphAgent == null)
        {
            Debug.LogError("BehaviorGraphAgent component is missing on " + GameObject.name);
            return Status.Failure;
        }

        rb = GameObject.GetComponent<Rigidbody2D>();

        stoppingDistance = navMeshAgent.stoppingDistance;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        BlackboardVariable<bool> canSeePlayer = new BlackboardVariable<bool>(false);
        behaviorGraphAgent.GetVariable<bool>("CanSeePlayer", out canSeePlayer);

        if(canSeePlayer.Value)
        {
            return Status.Success;
        }

        BlackboardVariable<bool> checkedPlayerLastPosition = new BlackboardVariable<bool>(false);
        behaviorGraphAgent.GetVariable<bool>("CheckedPlayerLastPosition", out checkedPlayerLastPosition);

        if (!isPatrolling)
        {
            if (!checkedPlayerLastPosition.Value)
            {
                BlackboardVariable<Vector2> playerLastPosition = new BlackboardVariable<Vector2>(Vector2.zero);
                behaviorGraphAgent.GetVariable<Vector2>("PlayerLastPosition", out playerLastPosition);
                checkingPosition = playerLastPosition.Value;
            }
            else
            {
                checkingPosition = entity.transform.position + new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0);
            }

            isPatrolling = true;
            navMeshAgent.stoppingDistance = 0;
            navMeshAgent.isStopped = false;
            patrolTimer = 0f;
        }

        if (patrolTimer > patrolTime)
        {
            isPatrolling = false;
            behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", true);
            return Status.Success;
        }

        PatrolTo(checkingPosition);
        patrolTimer += Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        isPatrolling = false;
        rb.linearVelocity = Vector2.zero;
        // reset the stopping distance
        navMeshAgent.stoppingDistance = stoppingDistance;
        if(navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }        
    }

    private void PatrolTo(Vector3 desiredPosition)
    {
        navMeshAgent.SetDestination(desiredPosition);
        Vector3 currentPosition = entity.transform.position;

        if (Vector3.Distance(currentPosition, desiredPosition) < 0.5f || !navMeshAgent.hasPath)
        {
            isPatrolling = false;
            behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", true);
            return;
        }

        //navMeshAgent.SetDestination(desiredPosition);
        //Debug.DrawRay(entity.transform.position, desiredPosition - entity.transform.position, Color.green);

        //// Move towards the desired position
        //if (navMeshAgent.remainingDistance > stoppingDistance + 1)
        //{
        //    Vector2 direction = (desiredPosition - entity.transform.position).normalized;
        //    rb.linearVelocity = direction * entity.GetRunSpeed() * Time.deltaTime;
        //    // Draw debug line
        //    Debug.DrawRay(entity.transform.position, direction * 10, Color.red);
        //    // Stop the agent after a while
        //    entity.StartCoroutine(StopAgent());
        //}
    }

    private IEnumerator StopAgent()
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;
    }
}

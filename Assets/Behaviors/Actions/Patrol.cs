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
    private Rigidbody2D rb;
    private float stoppingDistance = 0.5f;

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
            rb.linearVelocity = Vector2.zero;
            // reset the stopping distance
            navMeshAgent.stoppingDistance = stoppingDistance;
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
            }
            return Status.Success;
        }

        BlackboardVariable<bool> checkedPlayerLastPosition = new BlackboardVariable<bool>(false);
        behaviorGraphAgent.GetVariable<bool>("CheckedPlayerLastPosition", out checkedPlayerLastPosition);

        if (!checkedPlayerLastPosition.Value)
        {
            BlackboardVariable<Vector2> playerLastPosition = new BlackboardVariable<Vector2>(Vector2.zero);
            behaviorGraphAgent.GetVariable<Vector2>("PlayerLastPosition", out playerLastPosition);
            checkingPosition = playerLastPosition.Value;
            behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", true);
        }
        else
        {
            checkingPosition = GetRandomCheckingPosition();
        }

        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.isStopped = false;

        //if (patrolTimer > patrolTime)
        //{
        //    isPatrolling = false;
        //    behaviorGraphAgent.SetVariableValue<bool>("CheckedPlayerLastPosition", true);
        //    return Status.Success;
        //}

        //if(!navMeshAgent.hasPath)
        //{
        //    return Status.Success;
        //}

        PatrolTo(checkingPosition);
        return Status.Success;
    }

    private Vector2 GetRandomCheckingPosition()
    {
        Vector2 randomPosition = new Vector2(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return new Vector2(entity.transform.position.x, entity.transform.position.y);
    }

    protected override void OnEnd()
    {
        
    }

    private void PatrolTo(Vector3 desiredPosition)
    {
        DrawDebugDesiredPoint(desiredPosition);
        navMeshAgent.SetDestination(desiredPosition);
        Vector3 currentPosition = entity.transform.position;

        if (Vector3.Distance(currentPosition, desiredPosition) < 0.5f || !navMeshAgent.hasPath)
        {
            return;
        }

        //navMeshAgent.SetDestination(desiredPosition);
        //Debug.DrawRay(entity.transform.position, desiredPosition - entity.transform.position, Color.green);

        //// Move towards the desired position
        //if (navMeshAgent.remainingDistance > stoppingDistance + 1)
        //{
        //   Vector2 direction = (desiredPosition - entity.transform.position).normalized;
        //    rb.linearVelocity = direction * entity.GetRunSpeed() * Time.deltaTime;
        //    // Draw debug line
        //    Debug.DrawRay(entity.transform.position, direction * 10, Color.black);
        //    // Stop the agent after a while
        //    entity.StartCoroutine(StopAgent());
        //}
    }

    private IEnumerator StopAgent()
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;
    }

    void DrawDebugDesiredPoint(Vector3 desiredPosition)
    {
        Debug.DrawLine(entity.transform.position, desiredPosition, Color.green);
        DrawCircle(desiredPosition, 0.5f, Color.green);
    }

    void DrawCircle(Vector3 center, float radius, Color color, int segments = 36)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }
}

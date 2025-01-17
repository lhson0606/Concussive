using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Agent searches for random places or last-seen target's position", category: "Action", id: "b4277f60e0a7188e5162f27082dad714")]
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

        checkingPosition = entity.transform.position;

        entity.SafeDelegateOnCanMoveStateChanged(OnEntityCanMoveSateChange);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!entity.IsActivated() || !entity.CanMove())
        {
            navMeshAgent.isStopped = true;
            return Status.Success;
        }

        BlackboardVariable<bool> canSeeTarget = new BlackboardVariable<bool>(false);
        behaviorGraphAgent.GetVariable<bool>("CanSeeTarget", out canSeeTarget);

        if (canSeeTarget.Value)
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

        BlackboardVariable<bool> checkedTargetLastPosition = new BlackboardVariable<bool>(false);
        behaviorGraphAgent.GetVariable<bool>("CheckedTargetLastPosition", out checkedTargetLastPosition);

        if (!checkedTargetLastPosition.Value)
        {
            BlackboardVariable<Vector2> targetLastPosition = new BlackboardVariable<Vector2>(Vector2.zero);
            behaviorGraphAgent.GetVariable<Vector2>("TargetLastPosition", out targetLastPosition);
            checkingPosition = targetLastPosition.Value;
            behaviorGraphAgent.SetVariableValue<bool>("CheckedTargetLastPosition", true);
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
        //    behaviorGraphAgent.SetVariableValue<bool>("CheckedTargetLastPosition", true);
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
        Vector2 randomPosition = entity.transform.position + new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), 0);
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

    public void OnDrawGizmos()
    {
        DrawDebugDesiredPoint(checkingPosition);
    }

    private void PatrolTo(Vector3 desiredPosition)
    {
        //DrawDebugDesiredPoint(desiredPosition);
        entity.MovingToPosition = desiredPosition;
        if(!entity.CanMove())
        {
            return;
        }

        navMeshAgent.SetDestination(desiredPosition);
        Vector3 currentPosition = entity.transform.position;

        if (Vector3.Distance(currentPosition, desiredPosition) < 0.5f || !navMeshAgent.hasPath)
        {
            return;
        }

        //navMeshAgent.SetDestination(desiredPosition);
        //Debug.DrawRay(entity.transform.position, desiredPosition - entity.transform.position, Color.green);

        // Move towards the desired position
        if (navMeshAgent.remainingDistance > stoppingDistance + 3 && entity.IsPositionInSight(desiredPosition))
        {
            Vector2 direction = (desiredPosition - entity.transform.position).normalized;
            rb.linearVelocity = direction * entity.GetRunSpeed();
            // Draw debug line
            Debug.DrawRay(entity.transform.position, direction * 10, Color.black);
            // Stop the agent after a while
            entity.StartCoroutine(StopAgent());
        }
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

    public void OnEntityCanMoveSateChange(bool canMove)
    {
        if (!canMove && rb)
        {
            rb.linearVelocity = Vector2.zero;
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
            }
        }
    }
}

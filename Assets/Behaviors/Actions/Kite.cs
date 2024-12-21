using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Kite", story: "Try to move around to dodge target attack", category: "Action", id: "29b53629ebc4270ba78b44cfd602c66c")]
public partial class KiteAction : Action
{
    private Enemy entity;
    private NavMeshAgent navMeshAgent;
    private BehaviorGraphAgent behaviorGraphAgent;
    private Rigidbody2D rb;
    private float dodgeRange = 3f;
    private float originalStoppingDistance;

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

        originalStoppingDistance = navMeshAgent.stoppingDistance;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        navMeshAgent.stoppingDistance = 0;
        if(navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
        }
        

        Vector2 dodgePosition = GetRandomDodgePosition();

        if(!entity.CanSeePosition(dodgePosition))
        {
            return Status.Failure;
        }

        navMeshAgent.SetDestination(dodgePosition);

        DrawCircle(entity.transform.position, dodgeRange, Color.red);
        DrawCircle(dodgePosition, 0.5f, Color.blue);

        if (Vector3.Distance(entity.transform.position, dodgePosition) <= 1.0f)
        {
            rb.linearVelocity = Vector2.zero;
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.stoppingDistance = originalStoppingDistance;
            }
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
        
    }

    private Vector3 GetRandomDodgePosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle * dodgeRange;
        Vector3 dodgePosition = entity.transform.position + randomDirection;
        dodgePosition.z = 0;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dodgePosition, out hit, dodgeRange, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return entity.transform.position;
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


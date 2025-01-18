using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ApproachTarget", story: "Approach target", category: "Action", id: "f98ff2e03257b01226ffd08c29a54cbb")]
public partial class ApproachPlayerAction : Action
{
    private Enemy entity;
    private NavMeshAgent navMeshAgent;
    private BehaviorGraphAgent behaviorGraphAgent;
    private float stoppingDistance = 0.5f;
    private Rigidbody2D rb;

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

        stoppingDistance = 8f - 0.6f;
        rb = GameObject.GetComponent<Rigidbody2D>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!entity.HasTarget() || !entity.IsActivated() || !entity.CanMove())
        {
            return Status.Success;
        }
        navMeshAgent.isStopped = false;
        navMeshAgent.stoppingDistance = entity.AttackRange - 0.6f;

        navMeshAgent.SetDestination(entity.GetCurrentTarget().transform.position);

        if (navMeshAgent.remainingDistance > stoppingDistance + 3 && entity.IsTargetInSight())
        {
            Vector3 desiredPosition = entity.GetCurrentTarget().transform.position;
            Vector2 direction = (desiredPosition - entity.transform.position).normalized;
            rb.linearVelocity = direction * entity.GetRunSpeed();
            // Draw debug line
            Debug.DrawRay(entity.transform.position, direction * 10, Color.black);
            // Stop the agent after a while
            entity.StartCoroutine(StopAgent());
        }

        //if we are stucked and the target is reset, we should stop
        if (!entity.HasTarget())
        {
            return Status.Success;
        }

        if (entity.DistanceToCurrenTarget() <= entity.AttackRange)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    private IEnumerator StopAgent()
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;
    }

    protected override void OnEnd()
    {
        if(navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
    }
}


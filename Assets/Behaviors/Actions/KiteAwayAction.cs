using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "KiteAway", story: "Kite away from target for more space", category: "Action", id: "73c9daa940f6ffdab00a5ccff571f2e2")]
public partial class KiteAwayAction : Action
{
    private Enemy entity;
    private NavMeshAgent navMeshAgent;
    private BehaviorGraphAgent behaviorGraphAgent;
    private Rigidbody2D rb;
    private bool isKiting = false;

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
        navMeshAgent.stoppingDistance = 0;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(!entity.ShouldKiteAway() || !entity.HasTarget())
        {
            navMeshAgent.isStopped = true;
            return Status.Success;
        }

        Vector3 kitingPosition = entity.GetRandomKitingPosition();
        entity.MovingToPosition = kitingPosition;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(kitingPosition);

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}


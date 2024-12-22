using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ApproachTarget", story: "Approach target", category: "Action", id: "f98ff2e03257b01226ffd08c29a54cbb")]
public partial class ApproachPlayerAction : Action
{
    private Enemy entity;
    private NavMeshAgent navMeshAgent;
    private BehaviorGraphAgent behaviorGraphAgent;

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
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!entity.HasTarget() || !entity.IsActivated() || !entity.CanMove())
        {
            return Status.Success;
        }

        navMeshAgent.stoppingDistance = entity.AttackRange - 0.6f;

        navMeshAgent.SetDestination(entity.GetCurrentTarget().transform.position);

        if (entity.DistanceToCurrenTarget() <= entity.AttackRange + 0.5f)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}


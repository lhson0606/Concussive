using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "AttackPlayer", category: "Action", id: "02fff0ac5c6c6003372ea2ae1f5c6af4")]
public partial class AttackPlayerAction : Action
{
    private Enemy entity;

    protected override Status OnStart()
    {
        entity = GameObject.GetComponent<Enemy>();
        if (entity == null)
        {
            Debug.LogError("Enemy component is missing on " + GameObject.name);
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        entity.Attack();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}


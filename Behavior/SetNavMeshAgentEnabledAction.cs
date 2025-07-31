using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set NavMeshAgent Enabled", story: "[Self] sets NavMeshAgent active status to [Active] .", category: "Action/Navigation", id: "24c3ecaf7bfee43fc14298c0c1a61fc4")]
    public partial class SetNavMeshAgentEnabledAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<bool> Active;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out NavMeshAgent agent)) return Status.Failure;

            agent.enabled = Active;

            return Status.Success;
        }
    }


}

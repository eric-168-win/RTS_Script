using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Agent Avoidance", story: "Set [Agent] avoidance quality to [AvoidanceQuality]", category: "Action/Navigation", id: "87b4898cdb3b4c8ffb4b4d2b22b15512")]
    public partial class SetAgentAvoidanceAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<int> AvoidanceQuality;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out NavMeshAgent agent) || AvoidanceQuality > 4 || AvoidanceQuality < 0)
            {
                return Status.Failure;
            }
            agent.obstacleAvoidanceType = (ObstacleAvoidanceType)AvoidanceQuality.Value;
            return Status.Success;
        }

    }

}

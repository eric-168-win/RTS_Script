using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using RTS_LEARN.Utilities;

namespace RTS_LEARN.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop Agent", story: "[Agent] stops moving.", category: "Action/Navigation", id: "a6a9551b78128ab2b03edd0fd0077a27")]
    public partial class StopAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            if (Agent.Value.TryGetComponent(out NavMeshAgent agent))
            {
                if (agent.TryGetComponent(out Animator animator))
                {
                    animator.SetFloat(AnimationConstants.SPEED, 0);
                }

                agent.ResetPath();
                return Status.Success;
            }
            return Status.Failure;
        }
    }

}

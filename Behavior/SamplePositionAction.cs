using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Sample Position", story: "Set [TargetLocation] to the closest point on the NavMesh to [Target] .", category: "Action", id: "f1a2414653afe50d829ff46572f1e62e")]
    public partial class SamplePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<NavMeshAgent> Target;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(5);

        protected override Status OnStart()
        {
            if (Target.Value == null) return Status.Failure;

            NavMeshQueryFilter navMeshQueryFilter = new()
            {
                agentTypeID = Target.Value.agentTypeID,
                areaMask = Target.Value.areaMask
            };
            //can use for handling flying Units
            if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, SearchRadius, navMeshQueryFilter))
            //return true if the position is found !!!
            // if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, SearchRadius, NavMesh.AllAreas))
            {
                TargetLocation.Value = hit.position;
                return Status.Success;
            }

            return Status.Failure;
        }
    }


}

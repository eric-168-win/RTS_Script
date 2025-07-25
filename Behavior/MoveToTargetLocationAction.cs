using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace RTS_LEARN.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to Target Location", story: "[Agent] moves to [TargetLocation] .", category: "Action/Navigation", id: "62a5435881a959e964522b3e3b2b2f53")]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        private NavMeshAgent agent;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out agent))
            {
                return Status.Failure;
            }

            if (Vector3.Distance(agent.transform.position, TargetLocation.Value) <= agent.stoppingDistance)
            {
                return Status.Success;
            }

            agent.SetDestination(TargetLocation.Value);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            //OnUpdate is not deplayed a frame anymore => Add !agent.pathPending &&
            {
                return Status.Success;

            }
            return Status.Running;
        }

        // protected override void OnEnd()
        // //every time we exit the node//doesn't matter is success or failure
        // {
        // }
    }

}
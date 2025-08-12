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
    [NodeDescription(name: "Move to Target Location", story: "[Agent] moves to [TargetLocation] .", category: "Action/Navigation", id: "62a5435881a959e964522b3e3b2b2f53")]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        private NavMeshAgent agent;
        private Animator animator;

        protected override Status OnStart()
        {
            Debug.Log("aaa");
            if (!Agent.Value.TryGetComponent(out agent))
            {
                return Status.Failure;
            }
            Agent.Value.TryGetComponent(out animator);

            if (Vector3.Distance(agent.transform.position, TargetLocation.Value) <= agent.stoppingDistance)
            {
                return Status.Success;
            }
            Debug.Log("bbb");

            agent.SetDestination(TargetLocation.Value);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (animator != null)
            {
                animator.SetFloat(AnimationConstants.SPEED, agent.velocity.magnitude);
                // Animator.StringToHash("Speed") //Faster than "Speed"
            }
            Debug.Log("ccc");

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            //OnUpdate is not deplayed a frame anymore => Add !agent.pathPending &&
            {
                return Status.Success;

            }
            Debug.Log("ddd");

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (animator != null)
                animator.SetFloat(AnimationConstants.SPEED, 0);
        }
    }

}
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
    [NodeDescription(name: "Move To Target GameObject", story: "[Agent] moves to [TargetGameObject] .", category: "Action/Navigation", id: "77056e9a312991d67bd69398a273ae14")]
    public partial class MoveToTargetGameObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
        [SerializeReference] public BlackboardVariable<float> MoveThreshold = new(0.25f);

        private NavMeshAgent agent;
        private Animator animator;
        private Vector3 targetPosition;
        private Vector3 lastPosition;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out agent) || TargetGameObject.Value == null)
            {
                return Status.Failure;
            }
            Agent.Value.TryGetComponent(out animator);

            targetPosition = GetTargetPosition();
            if (Vector3.Distance(agent.transform.position, targetPosition) <= agent.stoppingDistance)
            {
                return Status.Success;
            }
            agent.SetDestination(targetPosition);
            lastPosition = targetPosition;
            return Status.Running;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 obj;
            if (TargetGameObject.Value.TryGetComponent(out Collider collider))
            {
                obj = collider.ClosestPoint(agent.transform.position);
            }
            else
            {
                obj = TargetGameObject.Value.transform.position;
            }
            return obj;
        }

        protected override Status OnUpdate()
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                return Status.Success;
            }

            Vector3 targetPosition = GetTargetPosition();
            if (Vector3.Distance(targetPosition, lastPosition) >= MoveThreshold)
            {
                agent.SetDestination(targetPosition);
                lastPosition = agent.destination;
                return Status.Running;
            }

            if (animator != null)
            {
                animator.SetFloat(AnimationConstants.SPEED, agent.velocity.magnitude);
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (animator != null)
                animator.SetFloat(AnimationConstants.SPEED, 0);
        }

    }
}
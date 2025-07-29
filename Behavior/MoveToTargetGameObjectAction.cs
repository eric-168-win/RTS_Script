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

        private NavMeshAgent agent;
        private Animator animator;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out agent) || TargetGameObject.Value == null)
            {
                return Status.Failure;
            }
            Agent.Value.TryGetComponent(out animator);

            Vector3 targetPosition = GetTargetPosition();
            if (Vector3.Distance(agent.transform.position, targetPosition) <= agent.stoppingDistance)
            {
                return Status.Success;
            }
            agent.SetDestination(targetPosition);
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
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                return Status.Success;
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
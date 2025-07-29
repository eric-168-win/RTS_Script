using RTS_LEARN.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Linq;
using RTS_LEARN.Utilities;

namespace RTS_LEARN.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to GatherableSupply", story: "[Agent] moves to [Supply] or nearby not busy supply.", category: "Action/Navigation", id: "52ffe1f67d50f1920bc1115e82f0f368")]
    public partial class MoveToGatherableSupplyAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);
        private NavMeshAgent agent;
        private LayerMask suppliesMask;
        private SupplySO supplySO;

        protected override Status OnStart()
        {
            suppliesMask = LayerMask.GetMask("Supplies");

            if (!HasValidInputs())
            {
                return Status.Failure;
            }

            Vector3 targetPosition = GetTargetPosition();
            agent.SetDestination(targetPosition);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Status.Running;
            }
            if (Supply.Value != null && !Supply.Value.IsBusy && Supply.Value.Amount > 0)
            {
                return Status.Success;
            }
            Collider[] colliders = FindNearByNotBusyColliders();

            if (colliders.Length > 0)
            {
                Array.Sort(colliders, new ClosestColliderComparer(agent.transform.position));
                Supply.Value = colliders[0].GetComponent<GatherableSupply>();
                agent.SetDestination(GetTargetPosition());
                return Status.Running;

            }

            return Status.Failure;
        }

        private Collider[] FindNearByNotBusyColliders()
        {
            return Physics.OverlapSphere(
                agent.transform.position,
                SearchRadius.Value,
                suppliesMask
                ).Where(collider =>
                collider.TryGetComponent(out GatherableSupply supply)
                    && !supply.IsBusy
                    && supply.Supply.Equals(supplySO)
                ).ToArray();
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 obj;
            if (Supply.Value.TryGetComponent(out Collider collider))
            {
                obj = collider.ClosestPoint(agent.transform.position);
            }
            else
            {
                obj = Supply.Value.transform.position;
            }
            return obj;
        }

        public bool HasValidInputs()
        {
            if (!Agent.Value.TryGetComponent(out agent) || (Supply.Value == null && supplySO == null))
            {
                return false;
            }

            if (Supply.Value != null)
            {
                supplySO = Supply.Value.Supply;
            }
            else
            {
                Collider[] colliders = FindNearByNotBusyColliders();
                if (colliders.Length > 0)
                {
                    Array.Sort(colliders, new ClosestColliderComparer(agent.transform.position));
                    Supply.Value = colliders[0].GetComponent<GatherableSupply>();
                }
                else
                {
                    return false;
                }
            }
            return true;

        }
    }
}
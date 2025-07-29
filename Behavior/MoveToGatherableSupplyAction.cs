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

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out agent))
            {
                return Status.Failure;
            }

            Vector3 targetPosition = GetTargetPosition();
            if (Vector3.Distance(agent.transform.position, targetPosition) <= agent.stoppingDistance)
            {
                return Status.Success;
            }
            suppliesMask = LayerMask.GetMask("Supplies");

            agent.SetDestination(targetPosition);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Status.Running;
            }
            if (!Supply.Value.IsBusy && Supply.Value.Amount > 0)
            {
                return Status.Success;
            }

            Collider[] colliders = Physics.OverlapSphere(
                agent.transform.position,
                SearchRadius.Value,
                suppliesMask
                ).Where(collider =>
                collider.TryGetComponent(out GatherableSupply supply)
                    && !supply.IsBusy
                    && supply.Supply.Equals(Supply.Value.Supply)
                ).ToArray();


            if (colliders.Length > 0)
            {
                Array.Sort(colliders, new ClosestColliderComparer(agent.transform.position));
                Supply.Value = colliders[0].GetComponent<GatherableSupply>();
                agent.SetDestination(GetTargetPosition());
                return Status.Running;

            }

            return Status.Failure;
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
    }
}
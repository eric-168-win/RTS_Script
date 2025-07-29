using RTS_LEARN.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace RTS_LEARN.Behavior
{


    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Gather Supplies", story: "[Unit] gathers [Amount] supplies from [GatherableSupplies] .", category: "Action/Units", id: "f2a16bce6f33689b582bc7c0e7f3ba24")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Unit;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> GatherableSupplies;

        private float enterTime;

        protected override Status OnStart()
        {
            if (GatherableSupplies.Value == null)
            {
                return Status.Failure;
            }
            enterTime = Time.time;
            GatherableSupplies.Value.BeginGather();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (GatherableSupplies.Value.Supply.BaseGatherTime + enterTime <= Time.time)
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (GatherableSupplies.Value == null) return;

            if (CurrentStatus == Status.Success)
            {
                Amount.Value = GatherableSupplies.Value.EndGather();
            }
            else
            {
                GatherableSupplies.Value.AbortGather();
            }
        }
    }
}
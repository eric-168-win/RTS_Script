using System;
using RTS_LEARN.Units;
using Unity.Behavior;
using UnityEngine;


namespace RTS_LEARN.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Building Is In Progress", story: "[BaseBuilding] is being built .", category: "Conditions", id: "c007792220653b54c45e8827352e80e8")]
    public partial class BuildingIsInProgressCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<BaseBuilding> BaseBuilding;

        public override bool IsTrue()
        {
            return BaseBuilding.Value != null
                && BaseBuilding.Value.Progress.State == BuildingProgress.BuildingState.Building;
        }
    }
}

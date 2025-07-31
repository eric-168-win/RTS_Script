using RTS_LEARN.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;


namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PickRandomLocationWithRendererBounds", story: "Set [TargetLocation] to a random point within [BuildingUnderConstruction] .", category: "Action", id: "1e75661bf27f3c226b78c0467f3ce96c")]
    public partial class PickRandomLocationWithRendererBoundsAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

        protected override Status OnStart()
        {
            if (BuildingUnderConstruction.Value == null
                || BuildingUnderConstruction.Value.MainRenderer == null) return Status.Failure;

            Renderer renderer = BuildingUnderConstruction.Value.MainRenderer;
            Bounds bounds = renderer.bounds;

            TargetLocation.Value = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                TargetLocation.Value.y,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );

            return Status.Success;
        }

    }

}

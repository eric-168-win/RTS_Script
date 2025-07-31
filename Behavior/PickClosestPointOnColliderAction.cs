using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Pick Closest Point on Collider", story: "Set [TargetLocation] to the closest point to [Target] on [Collider] .", category: "Action", id: "e7607c3d9cb19332eb3f7365668b934e")]
    public partial class PickClosestPointOnColliderAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<GameObject> Collider;

        protected override Status OnStart()
        {
            if (Target.Value == null || Collider.Value == null || !Collider.Value.TryGetComponent(out Collider collider))
            {
                return Status.Failure;
            }

            TargetLocation.Value = collider.ClosestPoint(Target.Value.transform.position);

            return Status.Success;

        }
    }
}


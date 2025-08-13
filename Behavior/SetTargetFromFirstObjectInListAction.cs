using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;


namespace RTS_LEARN.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Target from First Object in List", story: "Set [Target] to the first item in [List] .", category: "Action/Blackboard", id: "bf69ed75988ef66c02ddcef4b3c30d7e")]
    public partial class SetTargetFromFirstObjectInListAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<List<GameObject>> List;

        protected override Status OnStart()
        {
            if (List.Value == null || List.Value.Count == 0) return Status.Failure;

            Target.Value = List.Value[0];

            return Status.Success;
        }
    }
}

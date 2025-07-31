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
    [NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at [Speed] speed.", category: "Action/Navigation", id: "2c811632a5b7a155096f4fe52ea3c09d")]
    public partial class TranslatePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<float> Speed;
        private Animator animator;
        private float endTime;
        private Vector3 direction;
        private Transform selfTransform;

        protected override Status OnStart()
        {
            if (Self.Value == null) return Status.Failure;
            animator = Self.Value.GetComponent<Animator>();


            selfTransform = Self.Value.transform;
            float distance = Vector3.Distance(selfTransform.position, TargetLocation.Value);
            endTime = Time.time + distance / Speed;
            direction = (TargetLocation.Value - selfTransform.position).normalized;//unit vector

            selfTransform.forward = direction;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Time.time > endTime) return Status.Success;

            if (animator != null)
            {
                animator.SetFloat(AnimationConstants.SPEED, Speed);
            }

            selfTransform.position += Speed * Time.deltaTime * direction; //* Time.deltaTime //to make it frame rate independent
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (animator != null)
            {
                animator.SetFloat(AnimationConstants.SPEED, 0);
            }
        }

    }

}

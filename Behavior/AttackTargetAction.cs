using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using RTS_LEARN.Utilities;
using RTS_LEARN.Units;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack Target", story: "[Self] Attacks [Target] until it dies .", category: "Action", id: "ce5af0db9bffe225c8fa9a4de82bc495")]
    public partial class AttackTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;

        private NavMeshAgent navMeshAgent;
        private Transform selfTransform;
        private Animator animator;

        private IDamageable targetDamageable;
        private Transform targetTransform;

        private float lastAttackTime;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;

            selfTransform = Self.Value.transform;
            navMeshAgent = selfTransform.GetComponent<NavMeshAgent>();
            animator = selfTransform.GetComponent<Animator>();

            targetTransform = Target.Value.transform;
            targetDamageable = Target.Value.GetComponent<IDamageable>();

            if (animator != null)
            {
                animator.SetBool(AnimationConstants.ATTACK, true);
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null || targetDamageable.CurrentHealth == 0) return Status.Success;

            if (Vector3.Distance(targetTransform.position, selfTransform.position) >= AttackConfig.Value.AttackRange)
            {
                navMeshAgent.SetDestination(targetTransform.position);
                navMeshAgent.isStopped = false;
                return Status.Running;
            }

            navMeshAgent.isStopped = true;

            if (Time.time >= lastAttackTime + AttackConfig.Value.AttackDelay)
            {
                lastAttackTime = Time.time;
                targetDamageable.TakeDamage(AttackConfig.Value.Damage);
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (animator != null)
            {
                animator.SetBool(AnimationConstants.ATTACK, false);
            }
        }

        private bool HasValidInputs() => Self.Value != null && Self.Value.TryGetComponent(out NavMeshAgent _)
            && Target.Value != null && Target.Value.TryGetComponent(out IDamageable _)
            && AttackConfig.Value != null;
    }

}
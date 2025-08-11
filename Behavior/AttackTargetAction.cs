using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using RTS_LEARN.Utilities;
using RTS_LEARN.Units;
using System.Collections.Generic;

namespace RTS_LEARN.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack Target", story: "[Self] Attacks [Target] until it dies .", category: "Action", id: "ce5af0db9bffe225c8fa9a4de82bc495")]
    public partial class AttackTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
        [SerializeReference] public BlackboardVariable<List<GameObject>> NearbyEnemies;

        private AbstractUnit unit;
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
            unit = selfTransform.GetComponent<AbstractUnit>();

            targetTransform = Target.Value.transform;
            targetDamageable = Target.Value.GetComponent<IDamageable>();

            if (!NearbyEnemies.Value.Contains(Target.Value))
            {
                navMeshAgent.SetDestination(targetTransform.position);
                navMeshAgent.isStopped = false;
                if (animator != null)
                {
                    animator.SetBool(AnimationConstants.ATTACK, false);
                }
            }
            else
            {
                navMeshAgent.isStopped = true;
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null || targetDamageable.CurrentHealth == 0) return Status.Success;

            if (animator != null)
            {
                animator.SetFloat(AnimationConstants.SPEED, navMeshAgent.velocity.magnitude);
            }

            if (!NearbyEnemies.Value.Contains(Target.Value))
            {
                return Status.Running;
            }

            navMeshAgent.isStopped = true;

            // selfTransform.LookAt(targetTransform); 
            // //may break the 3D model if there are height elevation differences
            // not use because We only want the Y-axis to move

            Quaternion lookRotation = Quaternion.LookRotation(
                    (targetTransform.position - selfTransform.position).normalized,
                    Vector3.up
            );
            selfTransform.rotation = Quaternion.Euler(
                selfTransform.rotation.eulerAngles.x,
                lookRotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.z
            );

            if (animator != null)
            {
                animator.SetBool(AnimationConstants.ATTACK, true);
            }

            if (Time.time >= lastAttackTime + AttackConfig.Value.AttackDelay)
            {
                lastAttackTime = Time.time;
                if (unit.AttackingParticleSystem != null)
                {
                    unit.AttackingParticleSystem.Play();
                }
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
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = false;
            }
        }

        private bool HasValidInputs() => Self.Value != null && Self.Value.TryGetComponent(out NavMeshAgent _)
            && Self.Value.TryGetComponent(out AbstractUnit _)
            && Target.Value != null && Target.Value.TryGetComponent(out IDamageable _)
            && AttackConfig.Value != null && NearbyEnemies != null;
    }

}
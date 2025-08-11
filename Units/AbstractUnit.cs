using System.Collections.Generic;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.Utilities;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))] // Ensures NavMeshAgent is attached
    public abstract class AbstractUnit : AbstractCommandable, IMoveable, IAttacker
    {

        public float AgentRadius => agent.radius; //meaning only Getter// expression bodied property
        [field: SerializeField] public ParticleSystem AttackingParticleSystem { get; private set; }
        [SerializeField] private DamageableSensor DamageableSensor;
        private NavMeshAgent agent;
        protected BehaviorGraphAgent graphAgent;
        protected UnitSO unitSO;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            graphAgent = GetComponent<BehaviorGraphAgent>();
            graphAgent.SetVariableValue("Command", UnitCommands.Stop);

            unitSO = UnitSO as UnitSO;
            //UnitSO from AbstractCommandable.cs
            //cast UnitSO from AbstractUnitSO.cs to UnitSO.cs
            graphAgent.SetVariableValue("AttackConfig", unitSO.AttackConfig);
        }
        protected override void Start()
        {
            base.Start();
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));

            if (DamageableSensor != null)
            {
                DamageableSensor.OnUnitEnter += HandleUnitEnter;
                DamageableSensor.OnUnitExit += HandleUnitExit;
                DamageableSensor.SetupFrom(unitSO.AttackConfig);
            }

        }

        public void MoveTo(Vector3 position)
        {
            // agent.SetDestination(position);
            // SetCommandOverrides(null); // Clear commands
            graphAgent.SetVariableValue("TargetLocation", position);
            graphAgent.SetVariableValue("Command", UnitCommands.Move);//will abort current execution => place after TargetLocation

        }

        public void Stop()
        {
            // graphAgent.SetVariableValue("TargetLocation", transform.position);
            SetCommandOverrides(null); // Clear commands
            graphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        public void Attack(IDamageable damageable)
        {
            Debug.Log($"Attacking {damageable.Transform.name}");
            graphAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
            graphAgent.SetVariableValue("Command", UnitCommands.Attack);

        }

        public void Attack(Vector3 location)
        {
            graphAgent.SetVariableValue<GameObject>("TargetGameObject", null);
            graphAgent.SetVariableValue("TargetLocation", location);
            graphAgent.SetVariableValue("Command", UnitCommands.Attack);
        }


        private void HandleUnitEnter(IDamageable damageable)
        {
            List<GameObject> nearbyEnemies = SetNearbyEnemiesOnBlackboard();

            if (graphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> targetVariable)
                && targetVariable.Value == null && nearbyEnemies.Count > 0)
            {
                graphAgent.SetVariableValue("TargetGameObject", nearbyEnemies[0]);
            }
        }

        private void HandleUnitExit(IDamageable damageable)
        {
            List<GameObject> nearbyEnemies = SetNearbyEnemiesOnBlackboard();

            if (!graphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> targetVariable)
                || damageable.Transform.gameObject != targetVariable.Value) return;

            if (nearbyEnemies.Count > 0)
            {
                graphAgent.SetVariableValue("TargetGameObject", nearbyEnemies[0]);
            }
            else
            {
                graphAgent.SetVariableValue<GameObject>("TargetGameObject", null);
                graphAgent.SetVariableValue("TargetLocation", damageable.Transform.position);
            }
        }

        private List<GameObject> SetNearbyEnemiesOnBlackboard()
        {
            List<GameObject> nearbyEnemies = DamageableSensor.Damageables
                            .ConvertAll(damageable => damageable.Transform.gameObject);
            nearbyEnemies.Sort(new ClosestGameObjectComparer(transform.position));

            graphAgent.SetVariableValue("NearbyEnemies", nearbyEnemies);

            return nearbyEnemies;
        }

        protected virtual void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }
    }
}
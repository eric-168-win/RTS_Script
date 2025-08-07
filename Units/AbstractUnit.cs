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
        [SerializeField] private DamageableSensor DamageableSensor;
        private NavMeshAgent agent;
        protected BehaviorGraphAgent graphAgent;
        protected UnitSO unitSO;

        void Awake()
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
                DamageableSensor.OnUnitEnter += HandleUnitEnterOrExit;
                DamageableSensor.OnUnitExit += HandleUnitEnterOrExit;
                DamageableSensor.SetupFrom(unitSO.AttackConfig);
            }

        }

        public void MoveTo(Vector3 position)
        {
            // agent.SetDestination(position);
            SetCommandOverrides(null); // Clear commands
            graphAgent.SetVariableValue("TargetLocation", position);
            graphAgent.SetVariableValue("Command", UnitCommands.Move);//will abort current execution => place after TargetLocation

        }

        public void Stop()
        {
            // graphAgent.SetVariableValue("TargetLocation", transform.position);
            SetCommandOverrides(null); // Clear commands
            graphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }

        private void HandleUnitEnterOrExit(IDamageable damageable)
        {
            List<GameObject> nearbyEnemies = DamageableSensor.Damageables.ConvertAll(damageable => damageable.Transform.gameObject);
            nearbyEnemies.Sort(new ClosestGameObjectComparer(transform.position));

            graphAgent.SetVariableValue("NearbyEnemies", nearbyEnemies);

        }

        // private void HandleUnitExit(IDamageable damageable)
        // {
        //     graphAgent.SetVariableValue("NearbyEnemies",
        //     DamageableSensor.Damageables.ConvertAll(damageable => damageable.Transform.gameObject));
        // }

        public void Attack(IDamageable damageable)
        {
            Debug.Log($"Attacking {damageable.Transform.name}");
            graphAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
            graphAgent.SetVariableValue("Command", UnitCommands.Attack);

        }

    }
}
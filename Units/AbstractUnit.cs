using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))] // Ensures NavMeshAgent is attached
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {

        public float AgentRadius => agent.radius; //meaning only Getter// expression bodied property
        private NavMeshAgent agent;

        private BehaviorGraphAgent graphAgent;


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            graphAgent = GetComponent<BehaviorGraphAgent>();
        }
        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
            MoveTo(transform.position);
        }

        public void MoveTo(Vector3 position)
        {
            // agent.SetDestination(position);
            graphAgent.SetVariableValue("TargetLocation", position);
        }

        public void StopMoving()
        {
            throw new System.NotImplementedException();
        }

    }
}
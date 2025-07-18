using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Units
{
    [RequireComponent(typeof(NavMeshAgent))] // Ensures NavMeshAgent is attached
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {

        public float AgentRadius => agent.radius; //meaning only Getter// expression bodied property
        private NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            agent.SetDestination(position);
        }

        public void StopMoving()
        {
            throw new System.NotImplementedException();
        }

    }
}
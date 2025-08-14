using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Units
{
    public interface ITransportable
    {
        public Transform Transform { get; }
        public int TransportCapacityUsage { get; }
        public NavMeshAgent Agent { get; }
        
        public void LoadInto(ITransporter transporter);
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Behavior;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Units
{
    public class AirTransport : AbstractUnit, ITransporter
    {
        public int Capacity => unitSO.TransportConfig.Capacity;
        [field: SerializeField] public int UsedCapacity { get; private set; }

        private List<ITransportable> loadedUnits = new(8);
        public List<ITransportable> GetLoadedUnits() => loadedUnits.ToList();

        protected override void Start()
        {
            base.Start();

            if (graphAgent.GetVariable("LoadUnitEventChannel", out BlackboardVariable<LoadUnitEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleLoadUnit;
            }
        }

        public void Load(ITransportable unit)
        {
            if (UsedCapacity + unit.TransportCapacityUsage > Capacity) return;

            //graphAgent.SetVariableValue("TargetGameObject", unit.Transform.gameObject);
            if (graphAgent.GetVariable("LoadUnitTargets", out BlackboardVariable<List<GameObject>> loadUnitVariable))
            {
                loadUnitVariable.Value.Add(unit.Transform.gameObject);
                graphAgent.SetVariableValue("LoadUnitTargets", loadUnitVariable.Value);
            }

            graphAgent.SetVariableValue("Command", UnitCommands.LoadUnits);
        }

        public void Load(ITransportable[] units)
        {
            throw new NotImplementedException();
        }

        public bool Unload(ITransportable unit)
        {
            NavMeshQueryFilter queryFilter = new()
            {
                areaMask = unit.Agent.areaMask,
                agentTypeID = unit.Agent.agentTypeID
            };

            if (Physics.Raycast(//give a ray to cast from GO downwards to the floor
                    transform.position,
                    Vector3.down,
                    out RaycastHit raycastHit,
                    float.MaxValue,
                    unitSO.TransportConfig.SafeDropLayers)
                && NavMesh.SamplePosition(raycastHit.point, out NavMeshHit hit, 1, queryFilter))
            {
                UsedCapacity -= unit.TransportCapacityUsage;
                unit.Transform.SetParent(null);
                unit.Transform.position = hit.position;
                unit.Transform.gameObject.SetActive(true);
                unit.Agent.Warp(hit.position);

                if (unit is IMoveable moveable)
                {
                    moveable.MoveTo(hit.position);
                }

                loadedUnits.Remove(unit);
                return true;
            }

            return false;
        }

        public bool UnloadAll()
        {
            for (int i = loadedUnits.Count - 1; i >= 0; i--)
            {
                Unload(loadedUnits[i]);
            }

            return true;
        }

        private void HandleLoadUnit(GameObject self, GameObject targetGameObject)
        {
            targetGameObject.SetActive(false);
            targetGameObject.transform.SetParent(self.transform);
            ITransportable transportable = targetGameObject.GetComponent<ITransportable>();
            UsedCapacity += transportable.TransportCapacityUsage;

            loadedUnits.Add(transportable);

            if (graphAgent.GetVariable("LoadUnitTargets", out BlackboardVariable<List<GameObject>> loadUnitsVariable))
            {
                loadUnitsVariable.Value.Remove(targetGameObject);
                graphAgent.SetVariableValue("LoadUnitTargets", loadUnitsVariable.Value);
            }

            if (UsedCapacity >= Capacity)
            {
                graphAgent.SetVariableValue("Command", UnitCommands.Stop);
                graphAgent.SetVariableValue("LoadUnitTargets", new List<GameObject>(unitSO.TransportConfig.Capacity));//empty and reset
            }
        }
    }
}
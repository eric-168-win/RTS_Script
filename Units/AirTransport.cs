using System;
using System.Collections.Generic;
using RTS_LEARN.Behavior;
using Unity.Behavior;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public class AirTransport : AbstractUnit, ITransporter
    {
        public int Capacity => unitSO.TransportConfig.Capacity;
        [field: SerializeField] public int UsedCapacity { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (graphAgent.GetVariable("LoadUnitEventChannel", out BlackboardVariable<LoadUnitEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleLoadUnit;
            }
        }

        public List<ITransportable> GetLoadedUnits()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool UnloadAll()
        {
            throw new NotImplementedException();
        }
        private void HandleLoadUnit(GameObject self, GameObject targetGameObject)
        {
            targetGameObject.SetActive(false);
            targetGameObject.transform.SetParent(self.transform);
            ITransportable transportable = targetGameObject.GetComponent<ITransportable>();
            UsedCapacity += transportable.TransportCapacityUsage;

            if (graphAgent.GetVariable("LoadUnitTargets", out BlackboardVariable<List<GameObject>> loadUnitsVariable))
            {
                loadUnitsVariable.Value.Remove(targetGameObject);
                graphAgent.SetVariableValue("LoadUnitTargets", loadUnitsVariable.Value);
            }

            if (UsedCapacity >= Capacity)
            {
                graphAgent.SetVariableValue("Command", UnitCommands.Stop);
                graphAgent.SetVariableValue("LoadUnitTargets", new List<GameObject>(unitSO.TransportConfig.Capacity));
            }

        }
    }

}
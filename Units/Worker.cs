using System;
using RTS_LEARN.Behavior;
using RTS_LEARN.Environment;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using Unity.Behavior;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        public bool HasSupplies
        {
            get
            {
                if (graphAgent != null && graphAgent.GetVariable("SupplyAmountHeld", out BlackboardVariable<int> heldVariable))
                {
                    return heldVariable.Value > 0;
                }

                return false;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (graphAgent.GetVariable("GatherSuppliesEvent", out BlackboardVariable<GatherSuppliesEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleGatherSupplies;
            }
        }

        private void HandleGatherSupplies(GameObject self, int amount, SupplySO supply)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supply));
        }


        public void Gather(GatherableSupply supply)
        {
            graphAgent.SetVariableValue("Supply", supply);
            graphAgent.SetVariableValue("TargetGameObject", supply.gameObject);
            graphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }

        public GameObject Build(BuildingSO building, Vector3 targetLocation)
        {
            GameObject instance = Instantiate(building.Prefab, targetLocation, Quaternion.identity);
            if (instance.TryGetComponent(out BaseBuilding baseBuilding))
            {
                baseBuilding.ShowGhostVisuals();
            }
            else
            {
                Debug.LogError($"Missing BaseBuilding on Prefab for BuildingSO \"{building.name}\"! Cannot build!");
                return null;
            }
            // set up blackboard to build!
            graphAgent.SetVariableValue("BuildingSO", building);
            graphAgent.SetVariableValue("TargetLocation", targetLocation);
            graphAgent.SetVariableValue("Ghost", instance);
            graphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);

            return instance;
        }


        public void ReturnSupplies(GameObject commandPost)
        {
            graphAgent.SetVariableValue("CommandPost", commandPost);
            graphAgent.SetVariableValue("Command", UnitCommands.ReturnSupplies);
        }


    }
}
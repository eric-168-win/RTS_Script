using System;
using System.Diagnostics;
using RTS_LEARN.Behavior;
using RTS_LEARN.Commands;
using RTS_LEARN.Environment;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using Unity.Behavior;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        public bool IsBuilding =>
            graphAgent.GetVariable("Command", out BlackboardVariable<UnitCommands> command)
            && command.Value == UnitCommands.BuildBuilding;
        [SerializeField] private BaseCommand CancelBuildingCommand;
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

            if (graphAgent.GetVariable("BuildingEventChannel", out BlackboardVariable<BuildingEventChannel> buildingEventChannelVariable))
            {
                buildingEventChannelVariable.Value.Event += HandleBuildingEvent;
            }
        }

        private void HandleBuildingEvent(GameObject self, BuildingEventType eventType, BaseBuilding building)
        {
            switch (eventType)
            {
                case BuildingEventType.ArrivedAt:
                    if (building != null && building.Progress.State == BuildingProgress.BuildingState.Building)
                    {
                        Stop();
                        break;
                    }
                    SetCommandOverrides(new BaseCommand[] { CancelBuildingCommand });
                    break;
                case BuildingEventType.Begin:
                    SetCommandOverrides(new BaseCommand[] { CancelBuildingCommand });
                    break;
                case BuildingEventType.Cancel:
                case BuildingEventType.Abort:
                    SetCommandOverrides(null);
                    break;
                case BuildingEventType.Completed:
                default:
                    break;
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
            instance.name = "buggy_" + building.name;

            if (!instance.TryGetComponent(out BaseBuilding baseBuildingInGhost))
            {
                UnityEngine.Debug.Log($"Missing BaseBuilding on Prefab for BuildingSO \"{building.name}\"! Cannot build!");
                return null;
            }
            // set up blackboard to build!
            graphAgent.SetVariableValue("BuildingSO", building);
            graphAgent.SetVariableValue("TargetLocation", targetLocation);
            graphAgent.SetVariableValue("Ghost", instance);
            graphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);

            SetCommandOverrides(new BaseCommand[] { CancelBuildingCommand });
            Bus<SupplyEvent>.Raise(new SupplyEvent(-building.Cost.Minerals, building.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent(-building.Cost.Gas, building.Cost.GasSO));

            return instance;
        }

        public void ResumeBuilding(BaseBuilding building)
        {
            graphAgent.SetVariableValue("TargetLocation", building.transform.position);
            graphAgent.SetVariableValue("BuildingUnderConstruction", building);
            graphAgent.SetVariableValue("BuildingSO", building.BuildingSO);
            graphAgent.SetVariableValue<GameObject>("Ghost", null);
            graphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);

        }



        public void ReturnSupplies(GameObject commandPost)
        {
            graphAgent.SetVariableValue("CommandPost", commandPost);
            graphAgent.SetVariableValue("Command", UnitCommands.ReturnSupplies);
        }

        public void CancelBuilding()
        {
            if (graphAgent.GetVariable("Ghost", out BlackboardVariable<GameObject> ghostVariable)
                && ghostVariable.Value != null)
            {
                Destroy(ghostVariable.Value);
            }
            if (graphAgent.GetVariable("BuildingUnderConstruction", out BlackboardVariable<BaseBuilding> buildingVariable)
                && buildingVariable.Value != null)
            {
                AbstractUnitSO unitSO = buildingVariable.Value.BuildingSO;
                Bus<SupplyEvent>.Raise(new SupplyEvent(unitSO.Cost.Minerals, unitSO.Cost.MineralsSO));
                Bus<SupplyEvent>.Raise(new SupplyEvent(unitSO.Cost.Gas, unitSO.Cost.GasSO));
                Destroy(buildingVariable.Value.gameObject);//must add gameObject Not Just BaseBuilding
            }

            SetCommandOverrides(Array.Empty<BaseCommand>());
            Stop();
        }

    }
}
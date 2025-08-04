using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.UI.Containers;
using RTS_LEARN.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace RTS_LEARN.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private CommandsUI commandsUI;
        [SerializeField] private BuildingBuildingUI buildingBuildingUI;

        HashSet<AbstractCommandable> selectedUnits = new(12);


        void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            Bus<SupplyEvent>.OnEvent += HandleSupplyChange;
        }

        void Start()
        {
            commandsUI.Disable();
            buildingBuildingUI.Disable();
        }

        void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<SupplyEvent>.OnEvent -= HandleSupplyChange;

        }

        private void HandleSupplyChange(SupplyEvent evt)
        {
            commandsUI.EnableFor(selectedUnits);
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            selectedUnits.Remove(evt.Unit);
            RefreshUI();
        }


        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Add(commandable);
                commandsUI.EnableFor(selectedUnits);
            }

            if (selectedUnits.Count == 1 && evt.Unit is BaseBuilding building)
            {
                buildingBuildingUI.EnableFor(building);
            }
        }
        private void HandleUnitDeselected(UnitDeselectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Remove(commandable);
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (selectedUnits.Count > 0)
            {
                commandsUI.EnableFor(selectedUnits);
                if (selectedUnits.Count == 1 && selectedUnits.First() is BaseBuilding building)
                {
                    buildingBuildingUI.EnableFor(building);
                }
                else
                {
                    buildingBuildingUI.Disable();
                }
            }
            else
            {
                commandsUI.Disable();
                buildingBuildingUI.Disable();
            }
        }

    }

}
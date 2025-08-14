using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.Events;
using RTS_LEARN.UI.Containers;
using RTS_LEARN.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace RTS_LEARN.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private CommandsUI commandsUI;
        [SerializeField] private BuildingSelectedUI buildingSelectedUI;
        [SerializeField] private UnitIconUI unitIconUI;
        [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI;
        [SerializeField] private UnitTransportUI unitTransportUI;
        HashSet<AbstractCommandable> selectedUnits = new(12);

        void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            Bus<SupplyEvent>.OnEvent += HandleSupplyChange;
            Bus<UnitLoadEvent>.OnEvent += HandleLoadUnit;
            Bus<UnitUnloadEvent>.OnEvent += HandleUnloadUnit;

        }

        void Start()
        {
            commandsUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
            buildingSelectedUI.Disable();
            unitTransportUI.Disable();
        }

        void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<SupplyEvent>.OnEvent -= HandleSupplyChange;
            Bus<UnitLoadEvent>.OnEvent -= HandleLoadUnit;
            Bus<UnitUnloadEvent>.OnEvent -= HandleUnloadUnit;
        }

        private void HandleLoadUnit(UnitLoadEvent evt)
        {
            if (selectedUnits.Count == 1 && selectedUnits.First() is ITransporter)
            {
                RefreshUI();
            }
            else if (evt.Unit is AbstractCommandable commandable && selectedUnits.Contains(commandable))
            {
                commandable.Deselect(); // RefreshUI will be called because of the UnitDeselectedEvent raised from this.
            }
        }

        private void HandleUnloadUnit(UnitUnloadEvent evt)
        {
            if (selectedUnits.Count == 1 && selectedUnits.First() is ITransporter)
            {
                RefreshUI();
            }
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
                RefreshUI();
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

                if (selectedUnits.Count == 1)
                {
                    ResolveSingleUnitSelectedUI();
                }
                else
                {
                    unitIconUI.Disable();
                    singleUnitSelectedUI.Disable();
                    buildingSelectedUI.Disable();
                    unitTransportUI.Disable();
                }
            }
            else
            {
                DisableAllContainers();
            }
        }

        private void DisableAllContainers()
        {
            commandsUI.Disable();
            buildingSelectedUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
            unitTransportUI.Disable();
        }

        private void ResolveSingleUnitSelectedUI()
        {
            AbstractCommandable commandable = selectedUnits.First();
            unitIconUI.EnableFor(commandable);

            if (commandable is BaseBuilding building)
            {
                singleUnitSelectedUI.Disable();
                unitTransportUI.Disable();
                buildingSelectedUI.EnableFor(building);
            }
            else if (commandable is ITransporter transporter && transporter.UsedCapacity > 0)
            {
                unitTransportUI.EnableFor(transporter);
                buildingSelectedUI.Disable();
                singleUnitSelectedUI.Disable();
            }
            else
            {
                buildingSelectedUI.Disable();
                unitTransportUI.Disable();
                singleUnitSelectedUI.EnableFor(commandable);
            }
        }

    }
}


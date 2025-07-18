using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Commands;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.UI
{

    public class ActionsUI : MonoBehaviour
    {
        [SerializeField] private UIActionButton[] actionButtons;
        private HashSet<AbstractCommandable> selectedUnits = new(12);
        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;

            foreach (UIActionButton button in actionButtons)
            {
                button.SetIcon(null);
            }
        }
        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
        }

        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Add(commandable);
                RefreshButtons();
            }

        }

        private void HandleUnitDeselected(UnitDeselectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Remove(commandable);
                RefreshButtons();

            }
        }

        private void RefreshButtons()
        {
            HashSet<ActionBase> availableCommands = new(9);

            foreach (AbstractCommandable commandable in selectedUnits)
            {
                availableCommands.UnionWith(commandable.AvailableCommands);
            }

            for (int i = 0; i < actionButtons.Length; i++)
            {
                // if (i < availableCommands.Count)
                // {
                    ActionBase actionForSlot = availableCommands.Where(action => action.Slot == i).FirstOrDefault();
                    if (actionForSlot != null)
                    {
                        actionButtons[i].SetIcon(actionForSlot.Icon);
                    }
                    else
                    {
                        actionButtons[i].SetIcon(null);
                    }
                // }
            }
        }
    }
}
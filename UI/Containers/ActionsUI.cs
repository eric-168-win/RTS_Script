using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Commands;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.UI.Components;
using RTS_LEARN.Units;
using UnityEngine;
using UnityEngine.Events;

namespace RTS_LEARN.UI.Containers
{
    public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private UIActionButton[] actionButtons;
        private void Start()
        //wait for the UI to be ready
        {
            foreach (UIActionButton button in actionButtons)
            {
                button.Disable();
            }
        }

        public void EnableFor(HashSet<AbstractCommandable> selectedUnits)
        {
            RefreshButtons(selectedUnits);
        }

        public void Disable()
        {
            foreach (UIActionButton actionButton in actionButtons)
            {
                actionButton.Disable();
            }
        }

        private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
        {
            HashSet<ActionBase> availableCommands = new(9);

            foreach (AbstractCommandable commandable in selectedUnits)
            {
                availableCommands.UnionWith(commandable.AvailableCommands);
            }

            for (int i = 0; i < actionButtons.Length; i++)
            {
                ActionBase actionForSlot = availableCommands.Where(action => action.Slot == i).FirstOrDefault();
                if (actionForSlot != null)
                {
                    actionButtons[i].EnableFor(actionForSlot, HandleClick(actionForSlot));
                    // Debug.Log($"Enabling button for action: {actionForSlot.name} in slot {i}");
                }
                else
                {
                    actionButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClick(ActionBase Action)
        {
            return () => Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(Action));
        }

    }
}
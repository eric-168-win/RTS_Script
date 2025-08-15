using System;
using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Commands;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.UI.Components;
using RTS_LEARN.Units;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace RTS_LEARN.UI.Containers
{
    public class CommandsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private UICommandButton[] actionButtons;
        private void Start()
        //wait for the UI to be ready
        {
            foreach (UICommandButton button in actionButtons)
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
            foreach (UICommandButton actionButton in actionButtons)
            {
                actionButton.Disable();
            }
        }

        private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
        {
            IEnumerable<BaseCommand> availableCommands = selectedUnits.ElementAt(0).AvailableCommands;//[0] doesn't matter

            for (int i = 1; i < selectedUnits.Count; i++)
            {
                AbstractCommandable commandable = selectedUnits.ElementAt(i);
                if (commandable.AvailableCommands != null)
                {
                    availableCommands = availableCommands.Intersect(commandable.AvailableCommands);
                }
            }

            for (int i = 0; i < actionButtons.Length; i++)
            {
                BaseCommand actionForSlot = availableCommands.Where(action => action.Slot == i).FirstOrDefault();

                if (actionForSlot != null)
                {
                    actionButtons[i].EnableFor(actionForSlot, selectedUnits, HandleClick(actionForSlot));
                }
                else
                {
                    actionButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClick(BaseCommand Action)
        {
            return null;
            // return () => Bus<CommandSelectedEvent>.Raise(new CommandSelectedEvent(Action));
        }

    }
}
using System;
using System.Windows.Input;
using RTS_LEARN.Commands;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RTS_LEARN.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; }
        [field: SerializeField] public ActionBase[] AvailableCommands { get; private set; }
        [SerializeField] private DecalProjector decalProjector;
        private ActionBase[] initialCommands;

        [field: SerializeField] public AbstractUnitSO UnitSO { get; private set; }

        protected virtual void Start()
        {
            //protected => child classes can see and run
            //virtual => child classes can override this method
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            initialCommands = AvailableCommands;
        }


        public void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void Deselect()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }
            SetCommandOverrides(null);

            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void SetCommandOverrides(ActionBase[] commands)
        {
            if (commands == null || commands.Length == 0)
            {
                AvailableCommands = initialCommands;
            }
            else
            {
                AvailableCommands = commands;
            }
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

    }
}
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
        [SerializeField] private UnitSO UnitSO;

        protected virtual void Start()
        {
            //protected => child classes can see and run
            //virtual => child classes can override this method
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
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
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }
    }
}
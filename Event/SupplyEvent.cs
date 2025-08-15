using RTS_LEARN.EventBus;
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Event
{
    public struct SupplyEvent : IEvent
    {
        // public GameObject Self { get; }
        public int Amount { get; }
        public SupplySO Supply { get; }
        public Owner Owner { get; private set; }

        public SupplyEvent(Owner owner, int amount, SupplySO supply)
        {
            Amount = amount;
            Supply = supply;
            Owner = owner;
        }

    }
}

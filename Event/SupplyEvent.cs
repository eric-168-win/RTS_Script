using RTS_LEARN.EventBus;
using UnityEngine;

namespace RTS_LEARN.Event
{
    public struct SupplyEvent : IEvent
    {
        // public GameObject Self { get; }
        public int Amount { get; }
        public SupplySO Supply { get; }

        public SupplyEvent(int amount, SupplySO supply)
        {
            // Self = self;
            Amount = amount;
            Supply = supply;
        }
    }
}

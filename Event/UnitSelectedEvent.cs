using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct UnitSelectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitSelectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}
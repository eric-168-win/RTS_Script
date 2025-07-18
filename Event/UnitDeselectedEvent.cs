using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct UnitDeselectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitDeselectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}
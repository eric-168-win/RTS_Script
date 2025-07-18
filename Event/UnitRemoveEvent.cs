using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct UnitRemoveEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitRemoveEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
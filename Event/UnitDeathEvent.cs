using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct UnitDeathEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitDeathEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }

}
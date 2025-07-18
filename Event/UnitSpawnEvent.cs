using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct UnitSpawnEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitSpawnEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
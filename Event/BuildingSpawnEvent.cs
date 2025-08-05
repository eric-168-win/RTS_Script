using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct BuildingSpawnEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public BuildingSpawnEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
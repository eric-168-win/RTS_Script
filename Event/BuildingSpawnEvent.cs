using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building { get; private set; }

        public BuildingSpawnEvent(BaseBuilding building)
        {
            Building = building;
        }
    }
}
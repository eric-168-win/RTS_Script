using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Event
{
    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building { get; private set; }
        public Owner Owner { get; private set; }
        public BuildingSpawnEvent(Owner owner, BaseBuilding building)
        {
            Building = building;
            Owner = owner;
        }

    }
}
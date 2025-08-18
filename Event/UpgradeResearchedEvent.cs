using RTS_LEARN.EventBus;
using RTS_LEARN.TechTree;
using RTS_LEARN.Units;

namespace RTS_LEARN.Events
{
    public struct UpgradeResearchedEvent : IEvent
    {
        public Owner Owner { get; private set; }
        public UpgradeSO Upgrade { get; private set; }
        
        public UpgradeResearchedEvent(Owner owner, UpgradeSO upgrade)
        {
            Owner = owner;
            Upgrade = upgrade;
        }
    }
}

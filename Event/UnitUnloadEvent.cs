using RTS_LEARN.EventBus;
using RTS_LEARN.Units;

namespace RTS_LEARN.Events
{
    public struct UnitUnloadEvent : IEvent
    {
        public ITransportable Unit { get; private set; }
        public ITransporter Transporter { get; private set; }

        public UnitUnloadEvent(ITransportable unit, ITransporter transporter)
        {
            Unit = unit;
            Transporter = transporter;
        }
    }
}

using RTS_LEARN.Commands;
using RTS_LEARN.EventBus;

namespace RTS_LEARN.Event
{
    public class CommandSelectedEvent : IEvent
    {
        public BaseCommand Command { get; }

        public CommandSelectedEvent(BaseCommand command)
        {
            Command = command;
        }
    }
}
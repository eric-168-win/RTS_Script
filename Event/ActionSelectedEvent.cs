using RTS_LEARN.Commands;
using RTS_LEARN.EventBus;

namespace RTS_LEARN.Event
{
    public class ActionSelectedEvent : IEvent
    {
        public ActionBase Action { get; }

        public ActionSelectedEvent(ActionBase action)
        {
            Action = action;
        }
    }
}
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        public abstract bool CanHandle(CommandContext context);
        public abstract void Handle(CommandContext context);

    }
}
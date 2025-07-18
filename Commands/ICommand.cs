using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public interface ICommand
    {
        bool CanHandle(CommandContext context);
        void Handle(CommandContext context);
    }
}

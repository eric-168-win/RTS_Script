using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public interface ICommand
    {
        public bool IsSingleUnitCommand { get; }
        bool CanHandle(CommandContext context);
        void Handle(CommandContext context);
    }
}

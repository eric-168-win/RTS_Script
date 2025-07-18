using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public interface ICommand
    {
        bool CanHandle(AbstractCommandable commandable, RaycastHit hit);
        void Handle(AbstractCommandable commandable, RaycastHit hit);
    }
}

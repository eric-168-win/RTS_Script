using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        public abstract bool CanHandle(AbstractCommandable commandable, RaycastHit hit);
        public abstract void Handle(AbstractCommandable commandable, RaycastHit hit);

    }
}
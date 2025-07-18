using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public class MoveCommand : ScriptableObject, ICommand
    {
        public bool CanHandle(AbstractCommandable commandable, RaycastHit hit)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(AbstractCommandable commandable, RaycastHit hit)
        {
            throw new System.NotImplementedException();
        }

    }
}
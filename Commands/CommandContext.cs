using RTS_LEARN.Units;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace RTS_LEARN.Commands
{
    public struct CommandContext
    {
        public AbstractCommandable Commandable { get; private set; }
        public RaycastHit Hit { get; private set; }
        public int UnitIndex { get; set; } // Optional: if you need to track which unit is being commanded
        public MouseButton Button { get; private set; }
        public Owner Owner { get; private set; }
        public CommandContext(AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0, MouseButton mouseButton = MouseButton.Left)
        {
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
            Button = mouseButton;
            Owner = Owner.Player1;

        }

        public CommandContext(Owner owner, AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0, MouseButton mouseButton = MouseButton.Left)
        {
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
            Button = mouseButton;
            Owner = owner;
        }

    }
}
using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Commands
{
    public struct CommandContext
    {
        public AbstractCommandable Commandable { get; private set; }
        public RaycastHit Hit { get; private set; }

        public int UnitIndex { get; set; } // Optional: if you need to track which unit is being commanded

        public CommandContext(AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0)
        {
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
        }
    }
}